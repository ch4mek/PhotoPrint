import { RateUserItem } from '@/types/rates';

export const getExpandableParentIds = (items: RateUserItem[]) =>
	new Set(
		items
			.filter(({ projectRates }) => projectRates.length > 1)
			.map(({ user }) => user.id.toString())
	);
