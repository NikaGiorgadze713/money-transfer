function TransactionList({ transactions }) {
  return (
    <section>
      <h2>Transactions</h2>
      <table>
        <thead>
          <tr>
            <th>Date</th>
            <th>From</th>
            <th>To</th>
            <th>Amount</th>
          </tr>
        </thead>
        <tbody>
          {transactions.map((transaction) => (
            <tr key={transaction.id}>
              <td>{new Date(transaction.createdAtUtc).toLocaleString()}</td>
              <td>{transaction.fromOwner}</td>
              <td>{transaction.toOwner}</td>
              <td>{transaction.amount.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  )
}

export default TransactionList
