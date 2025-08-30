export interface WalletEntry {
  id: string;
  description: string;
  value: number;
  date: string;
  categories: string[];
}

export interface WalletEntries {
  items: WalletEntry[];
}
