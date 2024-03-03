import { toast } from "react-toastify";
import agent from "../../app/api/agent";
import useQuery from "../../app/util/hooks";
import { Button, Header, Icon, Segment } from "semantic-ui-react";

const RegisterSuccess = () => {
  const email = useQuery().get("email") as string;

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

  return (
    <Segment placeholder textAlign="center">
      <Header icon color="green">
        <Icon name="check" />
        <h1>Success Registering!</h1>
      </Header>
      <p>Please check your email (including spam) for the verification email</p>
      {email && (
        <>
          <p>Didn't receive the email? Please click the button below:</p>
          <Button
            className="ui button primary"
            onClick={handleConfirmEmailResend}
            content="Resend email"
            size="large"
          />
        </>
      )}
    </Segment>
  );
};

export default RegisterSuccess;
