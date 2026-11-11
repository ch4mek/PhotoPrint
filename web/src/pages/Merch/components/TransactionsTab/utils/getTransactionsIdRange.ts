export const getTransactionsIdRange = (
	transactionIdFrom: number,
	transactionIdTo: number
) =>
	transactionIdFrom !== transactionIdTo
		? `${transactionIdFrom} - ${transactionIdTo}`
		: `${transactionIdFrom}`;
