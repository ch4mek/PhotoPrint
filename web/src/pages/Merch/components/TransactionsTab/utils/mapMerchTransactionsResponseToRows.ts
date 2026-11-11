import dayjs from 'dayjs';

import { FIRST_ROWS_HEIGHT } from '../../ElementsTab/constants';
import { TransactionTypes } from '../constants';
import { MerchTransaction, MerchTransactionsGridRow } from '../types';

import { DATE_FORMAT } from '@/constants/date';

import { getTransactionsIdRange } from './getTransactionsIdRange';

export const mapMerchTransactionsResponseToRows = (data: MerchTransaction[]) =>
	data.map<MerchTransactionsGridRow>(
		({
			senderName,
			date,
			type,
			warehouseNames,
			transactionIdFrom,
			transactionIdTo,
			...rest
		}) => ({
			...rest,
			transactionIdFrom,
			transactionIdTo,
			orderTransactionId: getTransactionsIdRange(
				transactionIdFrom,
				transactionIdTo
			),
			storage: warehouseNames.join(', '),
			expanded: false,
			senderName,
			date: dayjs(date).format(DATE_FORMAT),
			type: TransactionTypes.find(({ id }) => id === type)?.name ?? '',
			itemsRowHeight: FIRST_ROWS_HEIGHT,
			color: '',
		})
	);
