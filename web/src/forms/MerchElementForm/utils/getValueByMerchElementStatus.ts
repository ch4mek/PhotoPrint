import { GetValueByMerchElementStatusParams } from '../types';

export const getValueByMerchElementStatus = <T>({
	isArchived,
	isPartOfAvailableKit,
	valueByStatus: { active, archived, archivedWithKit },
}: GetValueByMerchElementStatusParams<T>) => {
	if (isArchived) {
		return active;
	}

	if (isPartOfAvailableKit) {
		return archivedWithKit;
	}

	return archived;
};
