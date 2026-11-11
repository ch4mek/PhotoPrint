import { initialProfitAndLossFilters } from '../constants';
import { ProfitAndLossFilters } from '../types';

import { areArraysEqualByPrimitiveItems } from '@/helpers/areValuesEqual';

export const compareProfitAndLossFilters = ({
	id,
	user,
	department,
	project,
	client,
	legalEntity,
}: ProfitAndLossFilters) => ({
	id: id === initialProfitAndLossFilters.id,
	user: user === initialProfitAndLossFilters.user,
	department: areArraysEqualByPrimitiveItems(
		department,
		initialProfitAndLossFilters.department
	),
	project: project === initialProfitAndLossFilters.project,
	client: client === initialProfitAndLossFilters.client,
	legalEntity: legalEntity === initialProfitAndLossFilters.legalEntity,
});
