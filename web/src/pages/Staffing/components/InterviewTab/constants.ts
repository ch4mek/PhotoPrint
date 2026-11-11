import { TRACKED_TIME_DEFAULT_VALUE } from '../../../constants';

import { fromToDateObject } from '@/constants/defaultValues';
import { ThemeMode } from '@/theme';
import { GridColumnWidthsConfiguration } from '@/types/grid';
import {
	InterviewStatus,
	InterviewStatusColorInfo,
	InterviewType,
} from '@/types/interview';

export const DEFAULT_INTERVIEW_GRID_FILTERS = {
	id: '',
	user: '',
	specialization: '',
	client: '',
	type: [],
	date: fromToDateObject,
	time: TRACKED_TIME_DEFAULT_VALUE,
	status: [],
	saleManager: '',
	jiraTask: '',
	reasonForRefusal: '',
	note: '',
};

export const INITIAL_ORDER_BY_VALUE = 'date';

export const INTERVIEW_GRID_COLUMNS = {
	id: {
		key: 'id',
		name: 'Id',
		sortable: true,
	},
	specialization: {
		key: 'specialization',
		name: 'Specialization',
		sortable: true,
	},
	user: {
		key: 'user',
		name: 'User',
		sortable: true,
	},
	client: {
		key: 'clientName',
		name: 'Client',
		sortable: true,
	},
	type: {
		key: 'type',
		name: 'Type',
		sortable: true,
	},
	date: {
		key: 'date',
		name: 'Date',
		sortable: true,
	},
	time: {
		key: 'time',
		name: 'Time',
		sortable: true,
	},
	status: {
		key: 'statusText',
		name: 'Status',
		sortable: true,
	},
	saleManager: {
		key: 'saleManager',
		name: 'Sale manager',
		sortable: true,
	},
	jiraTask: {
		key: 'jiraTask',
		name: 'Jira task',
		sortable: true,
	},
	reasonForRefusal: {
		key: 'reasonForRefusal',
		name: 'Reason for refusal',
		sortable: false,
	},
	note: {
		key: 'note',
		name: 'Notes',
		sortable: false,
	},
};

export const getInterviewColumns = (): GridColumnWidthsConfiguration => ({
	id: 67,
	specialization: 130,
	user: 200,
	client: 186,
	type: 87,
	date: 100,
	time: 80,
	status: 154,
	saleManager: 200,
	jiraTask: 100,
	reasonForRefusal: 200,
	note: 260,
});

export const INTERVIEW_STATUS_OPTIONS = [
	{
		id: InterviewStatus.WaitingForInterview,
		description: 'Waiting for interview',
	},
	{
		id: InterviewStatus.WaitingForResult,
		description: 'Waiting for result',
	},
	{
		id: InterviewStatus.Failed,
		description: 'Failed',
	},
	{
		id: InterviewStatus.Success,
		description: 'Success',
	},
	{
		id: InterviewStatus.Started,
		description: 'Started',
	},
	{
		id: InterviewStatus.NotStarted,
		description: 'Not started',
	},
	{
		id: InterviewStatus.Canceled,
		description: 'Canceled',
	},
	{
		id: InterviewStatus.NoFeedback,
		description: 'No feedback',
	},
	{
		id: InterviewStatus.NoNextStepScheduled,
		description: 'No next step scheduled',
	},
];

export const INTERVIEW_TYPE_OPTIONS = [
	{ id: InterviewType.Prescreen, description: InterviewType.Prescreen },
	{ id: InterviewType.Tech, description: InterviewType.Tech },
	{ id: InterviewType.Final, description: InterviewType.Final },
	{ id: InterviewType.Other, description: InterviewType.Other },
];
export const interviewGrayStatusColor = {
	light: '#E0E3EF',
	dark: '#545968',
	nameColor: 'canceled',
} as const;

export const interviewInactiveGrayStatusColor = {
	light: '#F4F5F9',
	dark: '#424756',
} as const;

export const INTERVIEW_STATUS_COLORS = {
	waitingForInterview: {
		light: '#ffffff',
		dark: '#383d4d',
		nameColor: 'waitingForInterview',
	},
	waitingForResult: {
		light: '#FBEDBF',
		dark: '#C3A035',
		nameColor: 'waitingForResult',
	},
	success: {
		light: '#BFE1F3',
		dark: '#1280BE',
		nameColor: 'success',
	},
	started: {
		light: '#C4EBD6',
		dark: '#28AC6C',
		nameColor: 'started',
	},
	failed: {
		light: '#F6BFBF',
		dark: '#C2666B',
		nameColor: 'failed',
	},
	notStarted: {
		light: '#EF8C8C',
		dark: '#D33234',
		nameColor: 'notStarted',
	},

	canceled: interviewGrayStatusColor,
	noFeedback: interviewGrayStatusColor,
	noNextStepScheduled: interviewGrayStatusColor,
} as const satisfies Record<string, InterviewStatusColorInfo>;

export const INTERVIEW_INACTIVE_STATUS_COLORS: Record<
	InterviewStatus,
	Record<ThemeMode, string>
> = {
	[InterviewStatus.WaitingForInterview]: { light: '#ffffff', dark: '#383d4d' },
	[InterviewStatus.WaitingForResult]: {
		light: '#FEF9E9',
		dark: '#696045',
	},
	[InterviewStatus.Failed]: {
		light: '#FCE9E9',
		dark: '#684B58',
	},
	[InterviewStatus.Success]: {
		light: '#E9F5FB',
		dark: '#2B5475',
	},
	[InterviewStatus.Started]: {
		light: '#EAF8F1',
		dark: '#326458',
	},
	[InterviewStatus.NotStarted]: {
		light: '#F9D7D7',
		dark: '#6E3944',
	},
	[InterviewStatus.Canceled]: interviewInactiveGrayStatusColor,
	[InterviewStatus.NoFeedback]: interviewInactiveGrayStatusColor,
	[InterviewStatus.NoNextStepScheduled]: interviewInactiveGrayStatusColor,
} as const;

export const TODAY_CELL_COLOR = {
	light: '#f6d367',
	dark: '#ffdf2a',
};

export const INTERVIEW_ROW_CLASSES = {
	inActiveUser: 'inactive-user-row',
	dateToday: 'date-today',
	stageRow: 'stage-row',
	statusWaitingForInterview: 'status-waiting-for-interview',
	statusWaitingForResult: 'status-waiting-for-result',
	statusFailed: 'status-failed',
	statusSuccess: 'status-success',
	statusStarted: 'status-started',
	statusNotStarted: 'status-not-started',
	statusCanceled: 'status-canceled',
	statusNoFeedback: 'status-no-feedback',
	statusNoNextStepScheduled: 'status-no-next-step-scheduled',
};

export const STATUS_CLASS_PREFIX = 'status-';

export const MAIN_INTERVIEW_ROW_HEIGHT = 26;
export const STAGE_INTERVIEW_ROW_HEIGHT = 24;
export const STAGE_INTERVIEW_FONT_SIZE = '12px';

export const INTERVIEW_ENTITY_NAME = 'Interview';

export const INTERVIEW_DELETED_MESSAGE =
	'Interview has been successfully deleted';

export const INTERVIEW_DELETE_CONFIRMATION_MESSAGE =
	'Are you sure you want to delete the interview with';
