import { useState } from 'react'

function TransferForm({ accounts, onTransferComplete }) {
  const [fromAccountId, setFromAccountId] = useState('')
  const [toAccountId, setToAccountId] = useState('')
  const [amount, setAmount] = useState('')
  const [message, setMessage] = useState(null)
  const [submitting, setSubmitting] = useState(false)

  async function handleSubmit(event) {
    event.preventDefault()
    setSubmitting(true)
    setMessage(null)

    try {
      const response = await fetch('/api/transfers', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          fromAccountId: Number(fromAccountId),
          toAccountId: Number(toAccountId),
          amount: Number(amount),
        }),
      })

      const data = await response.json()

      if (response.ok) {
        setMessage({ text: data.message, type: 'success' })
        setAmount('')
        await onTransferComplete()
      } else {
        setMessage({ text: data.message, type: 'error' })
      }
    } catch {
      setMessage({ text: 'Could not reach the server.', type: 'error' })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <section>
      <h2>New Transfer</h2>
      <form onSubmit={handleSubmit}>
        <label>
          From account
          <select
            value={fromAccountId}
            onChange={(e) => setFromAccountId(e.target.value)}
            required
          >
            <option value="" disabled>
              Select account
            </option>
            {accounts.map((account) => (
              <option key={account.id} value={account.id}>
                {account.owner} (#{account.id})
              </option>
            ))}
          </select>
        </label>

        <label>
          To account
          <select
            value={toAccountId}
            onChange={(e) => setToAccountId(e.target.value)}
            required
          >
            <option value="" disabled>
              Select account
            </option>
            {accounts.map((account) => (
              <option key={account.id} value={account.id}>
                {account.owner} (#{account.id})
              </option>
            ))}
          </select>
        </label>

        <label>
          Amount
          <input
            type="number"
            min="0.01"
            step="0.01"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            required
          />
        </label>

        <button type="submit" disabled={submitting}>
          {submitting ? 'Sending...' : 'Send transfer'}
        </button>
      </form>

      {message && (
        <p className={message.type === 'success' ? 'message-success' : 'message-error'}>
          {message.text}
        </p>
      )}
    </section>
  )
}

export default TransferForm
