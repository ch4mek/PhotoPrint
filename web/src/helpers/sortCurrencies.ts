import { priorityCurrencyCodes } from '@/constants/currency';
import { CurrencyLookupItem } from '@/types/currency';

const getCurrencyDisplayOrderIndex = ({
	id,
	currencyCode,
}: CurrencyLookupItem) => {
	const code = currencyCode || id;
	const priorityIndex = priorityCurrencyCodes.indexOf(
		code as (typeof priorityCurrencyCodes)[number]
	);

	return priorityIndex === -1 ? priorityCurrencyCodes.length : priorityIndex;
};

export const sortCurrenciesByDisplayOrder = (
	currencies: CurrencyLookupItem[]
) =>
	[...currencies].sort((first, second) => {
		const orderDiff =
			getCurrencyDisplayOrderIndex(first) -
			getCurrencyDisplayOrderIndex(second);

		if (orderDiff !== 0) {
			return orderDiff;
		}

		return first.currencyCode.localeCompare(second.currencyCode);
	});
