import T from "./Translations";

export default function Welcome() {
  return (
    <>
      <h2>{T("welcome")}</h2>
      <div className="row">
        <div className="col-md-12">
          <img className="img-responsive" alt="Pets" src="images/pets.png" />
        </div>
      </div>
    </>
  );
}
