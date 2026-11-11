import dayjs from 'dayjs';

import {
	MAIN_INTERVIEW_ROW_HEIGHT,
	STAGE_INTERVIEW_ROW_HEIGHT,
} from '../constants';
import { InterviewGridRow } from '../types';

import { DATE_FORMAT } from '@/constants/date';
import {
	isChiefRole,
	isHeadOfDepartment,
	isGroupManager,
	Roles,
} from '@/constants/roles';
import { getFullName } from '@/helpers/utils';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';
import {
	GetInterviewDataResponse,
	InterviewData,
	InterviewType,
} from '@/types/interview';

import { getInterviewStatusLabel } from './getInterviewStatusLabel';

const createBaseRow = (
	item: InterviewData,
	handleOpen: (itemId?: string | number | undefined) => void,
	handleDelete: (
		itemId?: SimpleConfigurableConfigurationItem<string | number> | undefined
	) => void
) => {
	const userName = getFullName(
		item.userFirstName,
		item.userLastName,
		item.userFirstNameRu,
		item.userLastNameRu
	);
	const date = item.userInterviewDate ? dayjs(item.userInterviewDate) : null;

	return {
		id: item.id,
		user: userName,
		specialization: item.specialization || '',
		clientName: item.clientName || '',
		jiraTask: item.jiraTask || '',
		type: item.interviewType as InterviewType,
		statusText: getInterviewStatusLabel(item.status),
		date: date ? date.format(DATE_FORMAT) : null,
		time: item.userInterviewFromUtc?.slice(0, 5) || null,
		saleManager: getFullName(
			item.salesResponsibleUserFirstName || '',
			item.salesResponsibleUserLastName || '',
			item.salesResponsibleUserFirstNameRu,
			item.salesResponsibleUserLastNameRu
		),
		reasonForRefusal: item.reasonForRefusal || '',
		note: item.notes || '',
		handleEdit: (id?: string | number) => () => {
			handleOpen(id);
		},
		handleDelete: (id?: string | number) => () => {
			handleDelete({
				id: id ?? item.id,
				name: userName,
			});
		},
	};
};

const canViewPersonalInfo = (
	interview: InterviewData,
	currentUserRole: Roles,
	currentUserId: number,
	currentUserDepartmentId: string,
	currentUserResourceGroupId: string | null
) => {
	const {
		isUserActive,
		userPosition,
		userId,
		userDepartmentId,
		userResourceGroupId,
	} = interview;

	if (!isUserActive || isChiefRole(userPosition) || currentUserId === userId) {
		return false;
	}

	const isSameDepartment = currentUserDepartmentId === userDepartmentId;
	const isSameGroup =
		isSameDepartment && currentUserResourceGroupId === userResourceGroupId;

	if (isHeadOfDepartment(currentUserRole)) {
		return isSameDepartment;
	}

	if (isGroupManager(currentUserRole)) {
		return isSameGroup;
	}

	return isChiefRole(currentUserRole);
};

export const convertInterviewsDataIntoRows = (
	expandedIds: Set<number>,
	handleOpenMainInterview: (itemId?: string | number | undefined) => void,
	handleOpenInterviewStage: (itemId?: string | number | undefined) => void,
	handleDelete: (
		itemId?: SimpleConfigurableConfigurationItem<string | number> | undefined
	) => void,
	handleAddStage: (parentId: number) => void,
	data: GetInterviewDataResponse | undefined,
	currentUserId: number,
	currentUserRole: Roles,
	currentUserDepartmentId: string,
	currentUserResourceGroupId: string | null
): InterviewGridRow[] => {
	if (!data?.items || data.items.length === 0) {
		return [];
	}

	return data.items.flatMap((item) => {
		const base = item.main;
		const stages = item.stages || [];
		const isExpanded = expandedIds.has(base.id);

		const mainRow: InterviewGridRow = {
			...createBaseRow(base, handleOpenMainInterview, handleDelete),
			userId: base.userId,
			status: base.status || '',
			itemRowHeight: MAIN_INTERVIEW_ROW_HEIGHT,
			isMainInterview: true,
			hasStages: stages.length > 0,
			expanded: isExpanded,
			isActive: base.isUserActive,
			statusOrderWeight: base.statusOrderWeight,
			interviewTypeOrderWeight: base.interviewTypeOrderWeight,
			canViewPersonalInfo: canViewPersonalInfo(
				base,
				currentUserRole,
				currentUserId,
				currentUserDepartmentId,
				currentUserResourceGroupId
			),
			handleAddStage: (id?: string | number) => () => {
				handleAddStage(Number(id));
			},
		};

		const stageRows: InterviewGridRow[] = stages.map((stage) => ({
			...createBaseRow(stage, handleOpenInterviewStage, handleDelete),
			userId: base.userId,
			status: base.status || '',
			itemRowHeight: isExpanded ? STAGE_INTERVIEW_ROW_HEIGHT : 0,
			isMainInterview: false,
			hasStages: false,
			isActive: base.isUserActive,
			statusOrderWeight: stage.statusOrderWeight,
			interviewTypeOrderWeight: stage.interviewTypeOrderWeight,
			canViewPersonalInfo: false,

			specialization: '',
			user: '',
			clientName: '',
			saleManager: '',
			jiraTask: '',
		}));

		return [mainRow, ...stageRows];
	});
};
