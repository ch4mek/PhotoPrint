import { ManagementGridRow } from '../types';

import { formatToFixedPrecision } from '@/helpers/formatToFixedPrecision';
import { getDepartmentName } from '@/helpers/getDepartmentName';
import { getFullName } from '@/helpers/utils';
import {
	RATES_CHILD_ROW_HEIGHT,
	RATES_PARENT_ROW_HEIGHT,
} from '@/pages/ProfitAndLoss/constants';
import { DepartmentType } from '@/types/configuration';
import { ManagementProject, ManagementUserItem } from '@/types/management';

const getProjectRate = (project: ManagementProject) => {
	if (project.hourlyRate === null || project.hourlyRateCurrency === null) {
		return { rate: null, currency: null };
	}

	return {
		rate: project.hourlyRate,
		currency: project.hourlyRateCurrency.currencyCode,
	};
};

const getProjectIncome = (project: ManagementProject) => {
	if (project.income === null) {
		return { incomeAmount: null, incomeCurrency: null };
	}

	return {
		incomeAmount: project.income,
		incomeCurrency: project.hourlyRateCurrency?.currencyCode ?? null,
	};
};

const getChildRowId = (
	userId: number,
	project: ManagementProject,
	childIndex: number
) => `${userId}-${project.projectId}-${childIndex}`;

const getUniqueJoinedValues = (values: Array<string | null>) =>
	Array.from(new Set(values.filter(Boolean))).join(', ');

const formatMonthHoursCount = (hours: number | null) =>
	formatToFixedPrecision(hours ?? 0);

const getSharedCurrencyCode = (currencyCodes: Array<string | null>) => {
	if (currencyCodes.length === 0) {
		return null;
	}

	const [firstCurrencyCode, ...restCurrencyCodes] = currencyCodes;

	if (firstCurrencyCode === null) {
		return null;
	}

	const hasSameCurrency = restCurrencyCodes.every(
		(currencyCode) => currencyCode === firstCurrencyCode
	);

	return hasSameCurrency ? firstCurrencyCode : null;
};

export const convertManagementItemsIntoRows = (
	items: ManagementUserItem[],
	expandedIds: Set<string>,
	departmentTypes: DepartmentType[],
	canOpenProfile: (userId: number) => boolean
): ManagementGridRow[] => {
	if (items.length === 0) {
		return [];
	}

	return items.flatMap((item, index) => {
		const {
			id,
			firstName,
			lastName,
			firstNameRu,
			lastNameRu,
			departmentId,
			isActive,
			canUserInfoBeRetrievedByCurrentUser,
			projects,
			billableTrackedHoursAmount,
			hourlyRateAmount,
			incomeAmount,
			incomeBynAmount,
			salary,
			kpi,
			kpiColor,
		} = item;
		const userId = id.toString();
		const isExpanded = expandedIds.has(userId);
		const hasChildren = projects.length > 1;
		const canViewPersonalInfo =
			canOpenProfile(id) && canUserInfoBeRetrievedByCurrentUser;
		const sharedCurrency = getSharedCurrencyCode(
			projects.map(
				({ hourlyRateCurrency }) => hourlyRateCurrency?.currencyCode ?? null
			)
		);

		const parentRow: ManagementGridRow = {
			id: userId,
			userId: id,
			canViewPersonalInfo,
			parentId: null,
			isParent: true,
			itemRowHeight: RATES_PARENT_ROW_HEIGHT,
			expanded: isExpanded,
			hasChildren,
			isActive,
			disabled: !isActive,
			parentIndex: index,
			user: getFullName(firstName, lastName, firstNameRu, lastNameRu),
			department: getDepartmentName(departmentTypes, departmentId),
			projects: projects.map(({ projectName, projectColor }) => ({
				name: projectName,
				color: projectColor,
			})),
			client: getUniqueJoinedValues(
				projects.map(({ clientName }) => clientName)
			),
			legalEntity: getUniqueJoinedValues(
				projects.map(({ clientLegalEntity }) => clientLegalEntity)
			),
			rate: hourlyRateAmount,
			currency: sharedCurrency,
			incomeAmount,
			incomeCurrency: sharedCurrency,
			incomeBYN: incomeBynAmount,
			salary,
			kpi,
			kpiColor,
			monthHoursCount: formatMonthHoursCount(billableTrackedHoursAmount),
			expenses: null,
			profit: null,
		};

		if (!hasChildren) {
			return [parentRow];
		}

		const childRows: ManagementGridRow[] = projects.map(
			(project, childIndex) => {
				const { rate, currency } = getProjectRate(project);
				const { incomeAmount: projectIncomeAmount, incomeCurrency } =
					getProjectIncome(project);

				return {
					id: getChildRowId(id, project, childIndex),
					userId: id,
					canViewPersonalInfo: false,
					parentId: userId,
					isParent: false,
					itemRowHeight: isExpanded ? RATES_CHILD_ROW_HEIGHT : 0,
					expanded: false,
					hasChildren: false,
					isActive,
					disabled: !isActive,
					isLastChild: childIndex === projects.length - 1,
					parentIndex: index,
					user: '',
					department: '',
					projects: [
						{ name: project.projectName, color: project.projectColor },
					],
					client: project.clientName,
					legalEntity: project.clientLegalEntity ?? '',
					rate,
					currency,
					incomeAmount: projectIncomeAmount,
					incomeCurrency,
					incomeBYN: project.incomeByn,
					salary: null,
					kpi: null,
					kpiColor: null,
					monthHoursCount: formatMonthHoursCount(project.billableTrackedHours),
					expenses: null,
					profit: null,
				};
			}
		);

		return [parentRow, ...childRows];
	});
};
