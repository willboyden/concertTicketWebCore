import React from 'react';
import cities from './cities';
import {sortAs} from '../src/Utilities';
import TableRenderers from '../src/TableRenderers';
import createPlotlyComponent from 'react-plotly.js/factory';
import createPlotlyRenderers from '../src/PlotlyRenderers';
import PivotTableUI from '../src/PivotTableUI';
import '../src/pivottable.css';

const Plot = createPlotlyComponent(window.Plotly);

class PivotTableUISmartWrapper extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = {pivotState: props};
  }

  componentWillReceiveProps(nextProps) {
    this.setState({pivotState: nextProps});
  }

  render() {
    return (
      <PivotTableUI
        renderers={Object.assign(
          {},
          TableRenderers,
          createPlotlyRenderers(Plot)
        )}
        {...this.state.pivotState}
        onChange={s => this.setState({pivotState: s})}
        unusedOrientationCutoff={Infinity}
      />
    );
  }
}

export default class App extends React.Component {
  componentWillMount() {
    this.setState({
      mode: 'demo',
      filename: 'Sample Dataset: Tips',
      // textarea: 'Sample Dataset: Tips',
      pivotState: {
        data: cities,
        // rows: ['Payer Gender'],
        //  cols: ['Party Size'],
        //  aggregatorName: 'Sum over Sum',
        //   vals: ['Tip', 'Total Bill'],
        rendererName: 'Grouped Column Chart',
        // sorters: {
        //   Meal: sortAs(['Lunch', 'Dinner']),
        //   'Day of Week': sortAs(['Thursday', 'Friday', 'Saturday', 'Sunday']),
        // },
        plotlyOptions: {width: 900, height: 500},
        plotlyConfig: {},
        tableOptions: {
          clickCallback: function(e, value, filters, pivotData) {
            var names = [];
            pivotData.forEachMatchingRecord(filters, function(record) {
              names.push(record.Meal);
            });
            alert(names.join('\n'));
          },
        },
      },
    });
  }

  onClicked(textAreaValue) {
    // console.log(apiUrl.target.value);
    let apiUrl = textAreaValue.target.value;
    this.setState(
      {
        mode: 'thinking',
        filename: '(Parsing CSV...)',
        textarea: '',
        pivotState: {data: []},
      },
      () => {
        fetch('https://localhost:44350/api/testapi/GetCityVenues')
          .then(response => response.json())
          .then(result => {
            this.setState({
              mode: 'demo',
              //filename: files[0].name,
              pivotState: {data: result},
            });
          });
        // console.log(response.json());
      }
    );
  }

  onTextChange(textAreaValue) {
    this.setState({
      textarea: textAreaValue.target.value,
    });
  }

  render() {
    return (
      <div>
        <div className="row text-center">
          <div className="col-md-3 text-center">
            <p>...or paste some data:</p>
            <textarea
              value={this.state.textarea}
              onChange={this.onTextChange.bind(this)}
              // placeholder="Paste from apIurl ex -> https://localhost:44350/api/testapi/GetCityVenues"
            />

            <button
              value={this.state.textarea}
              onClick={this.onClicked.bind(this)}
            >
              Search Api
            </button>
          </div>
        </div>
        <div className="row text-center">
          <p>
            <em>Note: the data never leaves your browser!</em>
          </p>
          <br />
        </div>
        <div className="row">
          <h2 className="text-center">{this.state.filename}</h2>
          <br />

          <PivotTableUISmartWrapper {...this.state.pivotState} />
        </div>
      </div>
    );
  }
}
