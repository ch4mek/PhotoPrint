import { getInterviewStatusLabel } from '@/pages/Staffing/components/InterviewTab/utils/getInterviewStatusLabel';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';
import { InterviewType, InterviewStatus } from '@/types/interview';

import { InterviewFormValues } from './types';

export const FORM_TITLE = 'Interview form';

export const interviewTypes: SimpleConfigurableConfigurationItem<InterviewType>[] =
	[
		{ id: InterviewType.Prescreen, name: InterviewType.Prescreen },
		{ id: InterviewType.Tech, name: InterviewType.Tech },
		{ id: InterviewType.Final, name: InterviewType.Final },
		{ id: InterviewType.Other, name: InterviewType.Other },
	];

export const interviewStatuses: SimpleConfigurableConfigurationItem<InterviewStatus>[] =
	[
		{
			id: InterviewStatus.WaitingForInterview,
			name: getInterviewStatusLabel(InterviewStatus.WaitingForInterview),
		},
		{
			id: InterviewStatus.WaitingForResult,
			name: getInterviewStatusLabel(InterviewStatus.WaitingForResult),
		},
		{
			id: InterviewStatus.Failed,
			name: getInterviewStatusLabel(InterviewStatus.Failed),
		},
		{
			id: InterviewStatus.Success,
			name: getInterviewStatusLabel(InterviewStatus.Success),
		},
		{
			id: InterviewStatus.Started,
			name: getInterviewStatusLabel(InterviewStatus.Started),
		},
		{
			id: InterviewStatus.NotStarted,
			name: getInterviewStatusLabel(InterviewStatus.NotStarted),
		},
		{
			id: InterviewStatus.Canceled,
			name: getInterviewStatusLabel(InterviewStatus.Canceled),
		},
		{
			id: InterviewStatus.NoFeedback,
			name: getInterviewStatusLabel(InterviewStatus.NoFeedback),
		},
		{
			id: InterviewStatus.NoNextStepScheduled,
			name: getInterviewStatusLabel(InterviewStatus.NoNextStepScheduled),
		},
	];

export const initialFormValues: InterviewFormValues = {
	user: null,
	specialization: '',
	client: '',
	jiraTask: '',
	type: interviewTypes[1],
	status: interviewStatuses[0],
	date: null,
	time: '',
	saleManager: null,
	reasonForRefusal: '',
	note: '',
};
