import { managementGridRowFields } from '@/pages/ProfitAndLoss/components/ManagementTab/constants';

export type SummaryPriceField =
	| typeof managementGridRowFields.incomeBYN
	| typeof managementGridRowFields.salary;
