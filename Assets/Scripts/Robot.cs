using UnityEngine;
using Parse;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Robot : MonoBehaviour {

    private List<ParseUser> users = new List<ParseUser>();
    private List<ParseObject> choozy = new List<ParseObject>();
    private Task[] task = new Task[2];
    private bool done = false;
    private Graphic g;

    void Start() {
        g = GetComponent<Graphic>();
        ParseQuery<ParseObject> query = ParseObject.GetQuery("Choozy");
        query.WhereEqualTo("confidentiality", "0");
        query.WhereEqualTo("newS3Account", true);
        query.WhereNotEqualTo("deactivated", true);
        query.WhereGreaterThanOrEqualTo("createdAt", Time.realtimeSinceStartup);
        query.WhereLessThanOrEqualTo("numberOfVotes", 100);

        task[0] = ParseUser.Query.WhereEqualTo("isStar", true).FindAsync().ContinueWith(t => {
            IEnumerable<ParseUser> tmp = t.Result;
            foreach (ParseUser user in tmp)
                users.Add(user);
        });
        task[1] = query.FindAsync().ContinueWith(t => {
            IEnumerable<ParseObject> tmp = t.Result;
            foreach (ParseObject obj in tmp)
                choozy.Add(obj);
        });
    }

    void Update() {
        if (task[0].IsCompleted && task[1].IsCompleted && !done) {
            done = true;
            g.setTextViews(choozy);
            foreach (ParseObject post in choozy)
                managePost(post);
        }
    }

    private void managePost(ParseObject post) {
        int left = -1;
        try {
            left = post.Get<int>("option1Count");
        } catch (Exception e) {
            left = 0;
        }

        int all_votes = -1;
        try {
            all_votes = post.Get<int>("numberOfVotes");
        } catch (Exception e) {
            all_votes = 0;
        }

        int percL = 50;
        if (all_votes > 0)
            percL = (left * 100) / all_votes;

        if (percL > 100) percL = 100;
        if (percL < 0) percL = 0;

        int left_voteNb = (percL * users.Count) / 100;
        int right_voteNb = users.Count - left_voteNb;

        List<ParseUser> left_voters = new List<ParseUser>();
        List<ParseUser> all_users = users.ToList<ParseUser>();

        System.Random r = new System.Random();
        for (int i = 0; i < left_voteNb; i++)
        {
            int rnd = r.Next(all_users.Count);
            ParseUser user = all_users[rnd];

            left_voters.Add(user);
            all_users.Remove(user);
        }
        StartCoroutine(vote(post, left_voters, all_users));
    }

    private IEnumerator vote(ParseObject post, List<ParseUser> left, List<ParseUser> right) {
        while (left.Count > 0 || right.Count > 0) {
            string col = "option";
            string lk = "0";
            int rnd = UnityEngine.Random.Range(1, 3);
            float sec = UnityEngine.Random.Range(300.0f, 1800.0f);

            System.Random r = new System.Random();
            ParseUser user;
            if (rnd == 1 && left.Count > 0)
            {
                user = left[r.Next(left.Count)];
                left.Remove(user);
                col += "1Count";
            }
            else if (right.Count > 0)
            {
                user = right[r.Next(right.Count)];
                right.Remove(user);
                lk = "1";
                col += "2Count";
            }
            else
                continue;

            makeVote(post, col, lk, user);
            yield return new WaitForSeconds(sec);
        }
        yield return null;
    }

    private void makeVote(ParseObject obj, string col, string lk, ParseUser user) {
        int v = 0;
        try {
            v = obj.Get<int>(col) + 1;
        } catch (Exception e) {
            v++;
        }

        int all = 0;
        try {
            v = obj.Get<int>("numberOfVotes") + 1;
        } catch (Exception e) {
            all++;
        }

        obj.Increment(col);
        obj.Increment("numberOfVotes");
        obj.SaveAsync().ContinueWith(t => {
            ParseObject Like = new ParseObject("Participant");
            Like.SaveAsync().ContinueWith(tt => {
                Like["choozy"] = obj;
                Like["user"] = user;
                Like["choice"] = lk;
                Like["status"] = "Voted";
                Like.ACL = new ParseACL(user)
                {
                    PublicReadAccess = true,
                    PublicWriteAccess = false
                };

                Like.SaveAsync().ContinueWith(ttt => {
                    g.decrementVote(obj);
                });
            });
        });
    }
}