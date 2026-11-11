import { managementGridRowFields } from '@/pages/ProfitAndLoss/components/ManagementTab/constants';

export type PriceField =
	| typeof managementGridRowFields.incomeBYN
	| typeof managementGridRowFields.salary;
