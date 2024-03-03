import { useEffect, useState } from "react";
import { useStore } from "../../app/stores/store";
import useQuery from "../../app/util/hooks";
import agent from "../../app/api/agent";
import { toast } from "react-toastify";
import { Button, Header, Icon, Segment } from "semantic-ui-react";
import LoginForm from "./LoginForm";

const ConfirmEmail = () => {
  const { modalStore } = useStore();
  const email = useQuery().get("email") as string;
  const token = useQuery().get("token") as string;

  const Status = {
    Verified: "verified",
    Failed: "failed",
    Success: "success",
  };

  const [status, setStatus] = useState(Status.Verified);

  function handleConfirmEmailResend() {
    agent.Account.resendEmailVerification(email)
      .then(() => {
        toast.success("Verification email resent - please check your email");
      })
      .catch((error) => {
        console.log(error);
        toast.error("Verification email resent failed");
      });
  }

  useEffect(() => {
    if (email && token) {
      agent.Account.verifyEmail(email, token)
        .then(() => {
          setStatus(Status.Success);
        })
        .catch(() => {
          setStatus(Status.Failed);
        });
    } else {
      setStatus(Status.Failed);
    }
  }, [Status.Failed, Status.Success, email, token]);

  function getBody() {
    switch (status) {
      case Status.Verified:
        return <p>Verifying email...</p>;
      case Status.Failed:
        return (
          <div>
            <p>
              Verification failed - please check the link or request a new one
            </p>
            <Button
              primary
              onClick={handleConfirmEmailResend}
              content="Resend Email"
              size="huge"
            />
          </div>
        );
      case Status.Success:
        return (
          <div>
            <p> Email verified - you can now login</p>
            <Button
              primary
              size="huge"
              content="Login"
              onClick={() => modalStore?.openModal(<LoginForm />)}
            />
          </div>
        );
      default:
        return "Email verification status unknown";
    }
  }

  return (
    <Segment placeholder textAlign="center">
      <Header icon>
        <Icon name="envelope" />
        Email confirmation
      </Header>
      <Segment.Inline>{getBody()}</Segment.Inline>
    </Segment>
  );
};

export default ConfirmEmail;
