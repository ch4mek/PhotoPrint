import { LookupRecipientResponse } from '@/queries/merchTransactions/types';
import { AutoselectOption } from '@/types/common';
import { MerchStorageType } from '@/types/configuration';

import { getFullName } from './formatName';

export const getRecipientOptions = (
	isTransfer: boolean,
	storages: MerchStorageType[],
	users?: LookupRecipientResponse[]
): AutoselectOption[] =>
	isTransfer
		? storages.map(({ id, name }) => ({ id: Number(id), name }))
		: users?.map(({ id, firstName, lastName }) => ({
				id,
				name: getFullName(firstName, lastName),
		  })) ?? [];
