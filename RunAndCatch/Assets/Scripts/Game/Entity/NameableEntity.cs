
using Photon.Pun;
using UnityEngine;

public class NameableEntity : LivingEntity
{
    public TextMesh mNameLabel;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (mNameLabel != null)
        {
            if (Camera.main != null)
            {
                mNameLabel.transform.rotation = Camera.main.transform.rotation;
            }
        }
    }

    public string GetDisplayName()
    {
        return mNameLabel.text;
    }

    public void SetDisplayName(string displayName)
    {
        mNameLabel.text = displayName;
    }

    // Photon override
    /*
    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext(GetDisplayName());
        }
        else
        {
            SetDisplayName((string)stream.ReceiveNext());
        }
    }
    */
}
