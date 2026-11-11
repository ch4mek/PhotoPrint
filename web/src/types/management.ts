import { KpiColors } from '@/pages/Payload/types';
import { ByPageResponse } from '@/queries/types';
import { FinanceOrderingField } from '@/types/rates';

export type ManagementOrderingField =
	| FinanceOrderingField
	| 'TrackedHours'
	| 'Income'
	| 'IncomeByn'
	| 'Kpi';

interface HourlyRateCurrency {
	id: string;
	name: string;
	currencyCode: string;
	currencySymbol: string | null;
}

export interface ManagementProject {
	projectId: number;
	projectName: string;
	projectColor: string | null;
	clientId: number;
	clientName: string;
	clientLegalEntity: string | null;
	billableTrackedHours: number | null;
	income: number | null;
	hourlyRate: number | null;
	hourlyRateCurrency: HourlyRateCurrency | null;
	incomeByn: number | null;
}

export interface ManagementUserItem {
	id: number;
	firstName: string;
	lastName: string;
	firstNameRu: string | null;
	lastNameRu: string | null;
	departmentId: string | null;
	isActive: boolean;
	canUserInfoBeRetrievedByCurrentUser: boolean;
	projects: ManagementProject[];
	billableTrackedHoursAmount: number | null;
	hourlyRateAmount: number | null;
	incomeAmount: number | null;
	incomeBynAmount: number | null;
	salary: number | null;
	kpi: number | null;
	kpiColor: KpiColors | null;
}

export interface GetManagementResponse
	extends ByPageResponse<ManagementUserItem> {
	profit: number | null;
	expenses: number | null;
	incomeBynAmount: number | null;
	salaryAmount: number | null;
	averageKpi: number | null;
	averageKpiColor: KpiColors | null;
}
