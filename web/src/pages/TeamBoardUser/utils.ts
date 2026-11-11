import { css, Theme } from '@emotion/react';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';
import { DATE_FORMAT } from '@/constants/date';
import { POPUP_HEADER_HEIGHT } from '@/constants/popUp';
import { getMockRows } from '@/forms/EquipmentForm/EquipmentUsers/History/utils/getMockRows';
import {
	CONVERSATION_TYPE_LABELS,
	ConversationType,
	DevelopmentPlanPopUp,
} from '@/forms/TeamboardForm/Conversation/types';
import { UpdateUserSalary } from '@/forms/TeamboardForm/UserSalary/types';
import { formatDateString } from '@/helpers/dateToISOString';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import dayjs from '@/lib/dayjs';
import {
	UserDevelopmentPlan,
	DevelopmentPlanStatus,
	DevelopmentPlanRow,
	DevelopmentPlanRowContextMenuBindings,
} from '@/types/developmentPlan';
import { ConversationDocument } from '@/types/document';
import {
	UserConversation,
	UserFeedback,
	TeamboardTableMode,
	UserPerformanceReviewData,
} from '@/types/teamboard';

import { SalaryRow } from './components/SalaryTable/types';
import { DEFAULT_LOADER_ROWS_COUNT } from './constants';
import {
	ConversationRow,
	FeedbackRow,
	FeedbackType,
	PerformanceReviewRow,
	TeamBoardTabsId,
	TeamboardUserAccessLevel,
	TeamboardUserProfileKind,
} from './types';

export const convertConversationResponseToRows = (
	rows: Array<UserConversation>,
	handleEditClick: (id?: number | string) => () => void,
	handleViewClick?: (id?: number | string) => () => void
): Array<ConversationRow> =>
	rows.map(
		(row): ConversationRow => ({
			...row,
			conversationType:
				CONVERSATION_TYPE_LABELS[
					ConversationType[
						row.conversationType as keyof typeof ConversationType
					]
				] ??
				ConversationType[row.conversationType as keyof typeof ConversationType],
			conversationResult: row.conversationResult as FeedbackType | undefined,
			supervisor: row.supervisorFullName,
			date: formatDateString(row.date),
			handleEditClick,
			handleViewClick,
			files: row.odpConversationFiles || [],
			links: row.odpConversationDocumentLinks.map(({ link }) => link) || [],
		})
	);

export const getConversationLoaderRows = (
	rowsCount = DEFAULT_LOADER_ROWS_COUNT
) =>
	getMockRows(rowsCount, {
		agreements: '',
		date: '',
		employeeTheses: '',
		supervisor: '',
		theses: '',
		showSkeleton: true,
		files: [] as ConversationDocument[],
		documentLinks: [],
	});

export const convertDevelopmentPlanResponseToRows = (
	rows: Array<UserDevelopmentPlan>,
	{
		handleEditOptionClick,
		handleApproveOptionClick,
		handleRejectOptionClick,
		handleDeleteOptionClick,
	}: DevelopmentPlanRowContextMenuBindings
): Array<DevelopmentPlanRow> =>
	rows.map(({ id, startDate, deadlineDate, endDate, status, ...rest }) => ({
		...rest,
		id,
		startDate: formatDateString(startDate, ''),
		deadlineDate: formatDateString(deadlineDate, ''),
		endDate: formatDateString(endDate, ''),
		status: DevelopmentPlanStatus[status],
		handleEditClick: handleEditOptionClick,
		handleEditOptionClick: handleEditOptionClick?.(id),
		handleApproveOptionClick: handleApproveOptionClick?.(id, startDate),
		handleRejectOptionClick: handleRejectOptionClick?.(id),
		handleDeleteOptionClick: handleDeleteOptionClick?.(id, startDate),
	}));

export const getDevelopmentPlanLoaderRows = (
	rowsCount = DEFAULT_LOADER_ROWS_COUNT
) =>
	getMockRows(rowsCount, {
		actualResult: DevelopmentPlanStatus.None,
		deadlineDate: '',
		endDate: '',
		notes: '',
		startDate: '',
		summary: '',
		showSkeleton: true,
	});
export const convertFeedbackResponseToRows = (
	rows: Array<UserFeedback>,
	handleEditClick: (id?: number | string) => () => void
): Array<FeedbackRow> =>
	rows.map(
		({
			actionsTodo: actions,
			id,
			clientName: client,
			projectName: project,
			feedback,
			reviewer,
			conversationId,
			externalFeedbackType: type,
			externalFeedbackDate,
		}) => ({
			id,
			actions,
			client,
			project,
			feedback,
			reviewer,
			conversationId,
			type: type as FeedbackType,
			date: formatDateString(externalFeedbackDate, ''),
			handleEditClick,
		})
	);

export const getFeedbackLoaderRows = (rowsCount = DEFAULT_LOADER_ROWS_COUNT) =>
	getMockRows(rowsCount, {
		actions: '',
		client: '',
		conversationId: null,
		date: '',
		feedback: '',
		project: '',
		reviewer: '',
		type: FeedbackType.Normal,
		showSkeleton: true,
	});

export const mapDevelopmentPlanToRow = (
	item: DevelopmentPlanPopUp,
	onEditClick: (itemId?: string | number) => () => void
): DevelopmentPlanRow => {
	const { id, startDate, deadlineDate, endDate, notes, summary } = item;

	return {
		id,
		startDate: formatDateString(startDate, ''),
		deadlineDate: formatDateString(deadlineDate, ''),
		endDate: endDate ? formatDateString(endDate, '') : null,
		notes,
		summary,
		status: DevelopmentPlanStatus.None,
		handleEditClick: onEditClick,
	};
};

export const overrideTeamboardSectionTableStyles =
	(
		isPaginationShown: boolean,
		mode: TeamboardTableMode,
		columnIndex?: number,
		templateColumns?: string
	) =>
	(theme: Theme) =>
		css`
			${overrideGridStylesWithEditColumn(
				isPaginationShown,
				columnIndex,
				templateColumns
			)(theme)}

			${mode === TeamboardTableMode.Section &&
			css`
				.rdg-row {
					background-color: ${theme.palette['background-white']};
				}

				border-color: ${theme.palette['graphics-input-stroke']};
			`}
		`;

export const getNextIdInRows = <T extends { id: number }>(
	rows: T[]
): number => {
	if (rows.length === 0) {
		return 1;
	}

	return rows.reduce((maxId, row) => Math.max(maxId, row.id), 0) + 1;
};

export const mapUpdateSalaryResponseToSalaryRow = (
	item: UpdateUserSalary,
	onEditClick: (itemId?: string | number) => () => void
): SalaryRow => {
	const { id, date, salaryUsd } = item;

	return {
		id,
		salaryFrom: dayjs(date).format(DATE_FORMAT),
		salaryUsd: `${salaryUsd}$`,
		handleEditClick: onEditClick,
	};
};

export const convertPerformanceReviewToRows = (
	rows: Array<UserPerformanceReviewData>,
	handleEditClick: (itemId?: string | number) => () => void
): Array<PerformanceReviewRow> =>
	rows.map((row) => ({
		...row,
		performanceReviewDate: formatDateString(row.performanceReviewDate),
		handleEditClick,
		files: row?.odpConversationFiles || [],
		links: row?.odpConversationDocumentLinks?.map(({ link }) => link) || [],
	}));

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	container: {
		width: '90vw',
		height: '100%',

		'& > div': {
			height: `calc(100% - ${POPUP_HEADER_HEIGHT}px)`,
		},
	},
	content: {
		...getAdaptivePopUpContentStylesConfig({ theme, windowHeight }),
		padding: `${theme.spaces.m}px ${theme.spaces.l}px ${theme.spaces.l}px ${theme.spaces.l}px`,
	},
});

export const hasAccess = (
	accessLevel: TeamboardUserAccessLevel,
	requiredLevels: TeamboardUserAccessLevel[]
) => requiredLevels.includes(accessLevel);

export const isFullAccess = (accessLevel: TeamboardUserAccessLevel) =>
	accessLevel === TeamboardUserAccessLevel.Full;

export const isLimitedCoordinatorAccess = (
	accessLevel: TeamboardUserAccessLevel
) => accessLevel === TeamboardUserAccessLevel.LimitedCoordinator;

export const isLimitedAccountantAccess = (
	accessLevel: TeamboardUserAccessLevel
) => accessLevel === TeamboardUserAccessLevel.LimitedAccountant;

export const isLimitedHRAccess = (accessLevel: TeamboardUserAccessLevel) =>
	accessLevel === TeamboardUserAccessLevel.LimitedHR;

export const isInternProfile = (profileKind: TeamboardUserProfileKind) =>
	profileKind === TeamboardUserProfileKind.Intern;

export const isEmployeeProfile = (profileKind: TeamboardUserProfileKind) =>
	profileKind === TeamboardUserProfileKind.Employee;

const allowedTabIdsByProfileKind: Record<
	TeamboardUserProfileKind,
	readonly TeamBoardTabsId[] | null
> = {
	[TeamboardUserProfileKind.Employee]: null,
	[TeamboardUserProfileKind.Intern]: [
		TeamBoardTabsId.SUMMARY,
		TeamBoardTabsId.CONVERSATIONS,
		TeamBoardTabsId.SALARY_DETAILS,
	],
};

export const filterTabsByProfileKind = <T extends { id: TeamBoardTabsId }>(
	tabsList: T[],
	profileKind: TeamboardUserProfileKind
) => {
	const allowedTabIds = allowedTabIdsByProfileKind[profileKind];

	if (!allowedTabIds) {
		return tabsList;
	}

	return tabsList.filter((tab) => allowedTabIds.includes(tab.id));
};
