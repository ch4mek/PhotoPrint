import { ConvertRatesItemsIntoRowsParams, RatesGridRow } from '../types';

import { getDepartmentName } from '@/helpers/getDepartmentName';
import { getFullName } from '@/helpers/utils';
import {
	RATES_CHILD_ROW_HEIGHT,
	RATES_PARENT_ROW_HEIGHT,
} from '@/pages/ProfitAndLoss/constants';
import { ProjectRate } from '@/types/rates';

const emptyRate = { rate: null, currency: null };

const getProjectRate = (project: ProjectRate) => {
	if (project.hourlyRate === null || project.currencyId === null) {
		return emptyRate;
	}

	return {
		rate: project.hourlyRate,
		currency: project.currencyId,
	};
};

const getChildRowId = (userId: number, projectId: number, childIndex: number) =>
	`${userId}-${projectId}-${childIndex}`;

export const convertRatesItemsIntoRows = ({
	items,
	expandedIds,
	departmentTypes,
	canOpenProfile,
	handleOpenRateHistory,
}: ConvertRatesItemsIntoRowsParams): RatesGridRow[] => {
	if (items.length === 0) {
		return [];
	}

	return items.flatMap((item, index) => {
		const {
			user,
			projectRates,
			totalRate,
			currencyId,
			canUserInfoBeRetrievedByCurrentUser,
		} = item;
		const { isActive } = user;
		const userId = user.id.toString();
		const isExpanded = expandedIds.has(userId);
		const hasChildren = projectRates.length > 1;

		const canViewPersonalInfo =
			canOpenProfile(user.id) && canUserInfoBeRetrievedByCurrentUser;

		const parentRow: RatesGridRow = {
			id: userId,
			userId: user.id,
			parentId: null,
			projectId: hasChildren
				? null
				: projectRates[0]?.projectId.toString() ?? null,
			isParent: true,
			handleEditClick:
				handleOpenRateHistory && isActive && !hasChildren
					? handleOpenRateHistory
					: undefined,
			itemRowHeight: RATES_PARENT_ROW_HEIGHT,
			expanded: isExpanded,
			hasChildren,
			isActive,
			canViewPersonalInfo,
			disabled: !isActive,
			parentIndex: index,

			user: getFullName(
				user.firstName,
				user.lastName,
				user.firstNameRu,
				user.lastNameRu
			),
			department: getDepartmentName(departmentTypes, user.departmentId),
			projects: projectRates.map(({ projectName, projectColor }) => ({
				name: projectName,
				color: projectColor,
			})),
			client: Array.from(
				new Set(projectRates.map(({ clientName }) => clientName))
			).join(', '),
			legalEntity: Array.from(
				new Set(
					projectRates
						.map(({ clientLegalEntity }) => clientLegalEntity)
						.filter(Boolean)
				)
			).join(', '),
			rate: totalRate,
			currency: currencyId,
		};

		if (!hasChildren) {
			return [parentRow];
		}

		const childRows: RatesGridRow[] = projectRates.map(
			(project, childIndex) => {
				const projectRate = getProjectRate(project);

				return {
					id: getChildRowId(user.id, project.projectId, childIndex),
					userId: user.id,
					parentId: userId,
					projectId: project.projectId.toString(),
					isParent: false,
					handleEditClick:
						handleOpenRateHistory && isActive
							? handleOpenRateHistory
							: undefined,
					itemRowHeight: isExpanded ? RATES_CHILD_ROW_HEIGHT : 0,
					expanded: false,
					hasChildren: false,
					isActive,
					canViewPersonalInfo: false,
					disabled: !isActive,
					isLastChild: childIndex === projectRates.length - 1,
					parentIndex: index,

					user: '',
					department: '',
					projects: [
						{ name: project.projectName, color: project.projectColor },
					],
					client: project.clientName,
					legalEntity: project.clientLegalEntity ?? '',
					rate: projectRate.rate,
					currency: projectRate.currency,
				};
			}
		);

		return [parentRow, ...childRows];
	});
};
