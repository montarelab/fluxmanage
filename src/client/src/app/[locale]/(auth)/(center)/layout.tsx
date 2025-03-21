export default function CenteredLayout(props: { children: React.ReactNode }) {
  // const { userId } = null;

  // if (userId) {
  //   redirect("/dashboard");
  // }

  return (
    <div className="flex min-h-screen items-center justify-center">
      {props.children}
    </div>
  );
}
