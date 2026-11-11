import { InterviewStatus } from '@/types/interview';

export const getInterviewStatusLabel = (
	status: InterviewStatus | null
): string => {
	if (!status) {
		return '';
	}

	const statusLabels: Record<InterviewStatus, string> = {
		[InterviewStatus.WaitingForInterview]: 'Waiting for interview',
		[InterviewStatus.WaitingForResult]: 'Waiting for result',
		[InterviewStatus.Failed]: 'Failed',
		[InterviewStatus.Success]: 'Success',
		[InterviewStatus.Started]: 'Started',
		[InterviewStatus.NotStarted]: 'Not started',
		[InterviewStatus.Canceled]: 'Canceled',
		[InterviewStatus.NoFeedback]: 'No feedback',
		[InterviewStatus.NoNextStepScheduled]: 'No next step scheduled',
	};

	return statusLabels[status] || status;
};
