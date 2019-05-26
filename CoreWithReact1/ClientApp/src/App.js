import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Sinif } from './components/Sinif';
import { Ogrenci } from './components/Ogrenci';

export default class App extends Component {
  displayName = App.name

  render() {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/sinif' component={Sinif} />
        <Route path='/ogrenci' component={Ogrenci} />
      </Layout>
    );
  }
}
