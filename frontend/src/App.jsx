import { useEffect, useState } from 'react'
import AccountList from './components/AccountList'
import TransferForm from './components/TransferForm'
import TransactionList from './components/TransactionList'
import './App.css'

function App() {
  const [accounts, setAccounts] = useState([])
  const [transactions, setTransactions] = useState([])

  async function loadAccounts() {
    const response = await fetch('/api/accounts')
    setAccounts(await response.json())
  }

  async function loadTransactions() {
    const response = await fetch('/api/transactions')
    setTransactions(await response.json())
  }

  async function refresh() {
    await Promise.all([loadAccounts(), loadTransactions()])
  }

  useEffect(() => {
    refresh()
  }, [])

  return (
    <div className="app">
      <h1>Money Transfer</h1>
      <AccountList accounts={accounts} />
      <TransferForm accounts={accounts} onTransferComplete={refresh} />
      <TransactionList transactions={transactions} />
    </div>
  )
}

export default App
