import React, { Component } from 'react';
import "react-table/react-table.css";
import { MDBCol } from "mdbreact";

export class Ogrenci extends Component {
    displayName = Ogrenci.name

    constructor(props) {
        super(props);
        this.state = {
            data: { item1: 0, item2: [{ ad:"", soyad:"", numara:"" }], item3: [{ s_adi: "" }], item4: [{ ders_Adi: "" }] },
            searchText: "",
            searchTextFind: "",
            loading: true,
            pageNo: 0
        }
    }

    componentDidMount() {
        this.sendPageRequest()  // load data
    }

    // veri tabanina kacinci sayfayi istedigini ve aranan kelimeyi yolla
    sendPageRequest() {
        console.log("gönderilmesi gereken bilgiler:" + this.state.pageNo + "-" + this.state.searchTextFind)
        console.log("INCELEME:" + JSON.stringify({ PageNo: this.state.pageNo, Search: this.state.searchTextFind }))
        fetch("api/SampleData/postOgrenci", {
            method: "POST",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ PageNo: this.state.pageNo, Search: this.state.searchTextFind })
        }).then(res => res.json())
            .then(data => {
                //gelen veriler  - toplam kayit sayisi, sayfa sayisi, tablo verileri
                console.log(data);
                this.setState({ data: data, loading: false })
                console.log("INCELEME_VERI:" + this.state.data.item1)
            })
    }

    // search input alanini searchText state ata
    handleChange(event) {
        this.setState({ searchText: event.target.value })
        console.log("veri:" + this.state.searchText)
    }

    // Ara butonu tiklandiginda - butona tiklandiginda servera yollanicak olan veriye esitle.
    onClickbutton() {
        console.log("Clicked" + this.state.searchText)
        this.setState({
            searchTextFind: this.state.searchText,
            pageNo: 0
        }, () => this.sendPageRequest())
    }

    // Sonraki butonu tiklandiginda - Sayfa sayisi toplam kayit sayisini asmasin. Sayfa sayisini 1 arttir serverdan veri cek. 
    onClickBtnNext() {
        if (Number(this.state.data.item1) > (this.state.pageNo + 1) * 10) {
            this.setState({ pageNo: this.state.pageNo + 1 }, () => this.sendPageRequest())
            console.log("Sonraki:" + this.state.searchText + "-" + this.state.pageNo)
        }
    }

    // Geri butonu tiklandiginda - sayfa sayisi 0 ise azaltma islemi yapma!
    onClickBtnBack() {
        if (this.state.pageNo != 0) {
            this.setState({ pageNo: this.state.pageNo - 1 }, () => this.sendPageRequest())
            console.log("Sonraki:" + this.state.searchText + "-" + this.state.pageNo)
        }
    }

    static renderTable(data) {
        return (
            <table className='table'>
                <thead>
                    <tr>
                        <th>Ad</th>
                        <th>Soyad</th>
                        <th>Numara</th>
                        <th>Sinif</th>
                        <th>Ders</th>
                    </tr>
                </thead>
                <tbody>
                    {data.item2.map((dt, index) =>
                        <tr key={index}>
                            <td>{dt.ad}</td>
                            <td>{dt.soyad}</td>
                            <td>{dt.numara}</td>
                            <td>{data.item3[index].s_adi}</td>
                            <td>{data.item4[index].ders_Adi}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }


    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Ogrenci.renderTable(this.state.data);

        return (
            <div>
                <div className="col-md-12">
                    <div className="row">
                        <h1 className="text-center">Ogrenci Listesi</h1>
                    </div>
                    <div className="row">
                        <MDBCol md="6">
                            <input className="form-control" type="text" placeholder="Search" value={this.state.searchText} aria-label="Search" onChange={this.handleChange.bind(this)} />
                        </MDBCol>
                        <button onClick={() => this.onClickbutton()} type="button" className="btn btn-primary btn-md">Ara</button>
                    </div>
                    <div className="row">
                        {contents}
                    </div>
                    <div className="row">
                        <button onClick={() => this.onClickBtnBack()} type="button" className="btn btn-default"> Geri </button>
                        <button onClick={() => this.onClickBtnNext()} type="button" className="btn btn-default">Sonraki</button>
                        <p className="text-right">{this.state.data.item1} kayit bulundu.</p>
                        <p className="text-right">{this.state.pageNo + 1}/{Math.floor(Number(this.state.data.item1) / 10) + 1}</p>
                    </div>
                </div>
            </div>
        );
    }
}