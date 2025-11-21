
export default function OwnerCreateForm() {
  return (<div>
    <h2>Owner</h2>
      <form className="form-horizontal" id="add-owner-form" method="post">
        <div className="form-group has-feedback">
          
          <div className="form-group has-error">
            <label htmlFor="firstName" className="col-sm-2 control-label">First Name</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="firstName" name="firstName" value="" />
              </div>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                <span className="help-inline">must not be blank</span>              
            </div>
          </div>
          
          <div className="form-group has-error">
            <label htmlFor="lastName" className="col-sm-2 control-label">Last Name</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="lastName" name="lastName" value="" />                
              </div>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                <span className="help-inline">must not be blank</span>              
            </div>
          </div>        
          
          <div className="form-group has-error">
            <label htmlFor="address" className="col-sm-2 control-label">Address</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="address" name="address" value="" />
              </div>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                <span className="help-inline">must not be blank</span>              
            </div>
          </div>
          
          <div className="form-group has-error">
            <label htmlFor="city" className="col-sm-2 control-label">City</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="city" name="city" value="" />
              </div>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                <span className="help-inline">must not be blank</span>
            </div>
          </div>
          
          <div className="form-group has-error">
            <label htmlFor="telephone" className="col-sm-2 control-label">Telephone</label>
            <div className="col-sm-10">
              <div>
                <input className="form-control" type="text" id="telephone" name="telephone" value="" />                
              </div>
                <span className="fa fa-remove form-control-feedback" aria-hidden="true"></span>
                <span className="help-inline">Telephone must be a 10-digit number<br />must not be blank</span>              
            </div>
          </div>
        
        </div>
        <div className="form-group">
          <div className="col-sm-offset-2 col-sm-10">
            <button className="btn btn-primary" type="submit">Add Owner</button>
          </div>
        </div>
      </form>
    </div>);
}
