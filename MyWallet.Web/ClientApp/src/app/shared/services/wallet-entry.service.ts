import {Inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {WalletEntries, WalletEntry} from "../models/wallet-entry";

@Injectable({
  providedIn: 'root'
})
export class WalletEntryService {

  private readonly apiUrl: string;
  constructor(private http: HttpClient,@Inject('BASE_URL') private baseUrl: string) {
    this.apiUrl = `${this.baseUrl}api/WalletEntry`;
  }

  public getWalletEntries(): Observable<WalletEntries> {
    return this.http.get<any>(this.apiUrl);
  }

  public createWalletEntry(entry: WalletEntry): Observable<WalletEntry> {
    return this.http.post<WalletEntry>(this.apiUrl, entry);
  }

  public getWalletEntryById(id: string): Observable<WalletEntry> {
    return this.http.get<WalletEntry>(`${this.apiUrl}/${id}`);
  }

  public updateWalletEntry(id: string, entry: WalletEntry): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, entry);
  }

  public deleteWalletEntry(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

}
