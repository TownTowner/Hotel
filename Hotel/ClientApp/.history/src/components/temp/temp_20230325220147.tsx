import React, { Component } from 'react';

export class Temp extends Component {
  static displayName = Temp.name;
  state = {
    currentCount: 0,
  };

  constructor(props:any) {
    super(props);
    // this.setState({ currentCount: 0 });
    // this.incrementCounter = this.incrementCounter.bind(this);
  }

  incrementCounter() {
    this.setState({
      currentCount: this.state.currentCount + 1
    });
    console.log('sss');
  }

  render() {
    return (
      <div>
        <h1>Temp</h1>

        <p>This is a simple example of a React component.</p>

        <p aria-live="polite">Current count: <strong>{this.state.currentCount}</strong></p>

        <button className="btn btn-primary" onClick={this.incrementCounter}>Increment</button>
      </div>
    );
  }
}
