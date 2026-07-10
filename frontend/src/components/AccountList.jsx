function AccountList({ accounts }) {
  return (
    <section>
      <h2>Accounts</h2>
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Owner</th>
            <th>Balance</th>
          </tr>
        </thead>
        <tbody>
          {accounts.map((account) => (
            <tr key={account.id}>
              <td>{account.id}</td>
              <td>{account.owner}</td>
              <td>{account.balance.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  )
}

export default AccountList
