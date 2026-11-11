import { PlanCompletionStatus } from '@/pages/Payload/components/WorkloadCell/constants';

export const getPlanCompletion = (firstTime: number, secondTime: number) => {
	if (firstTime === 0 && secondTime === 0) {
		return PlanCompletionStatus.Zero;
	}

	if (firstTime === secondTime) {
		return PlanCompletionStatus.Completed;
	}

	if (firstTime > secondTime) {
		return PlanCompletionStatus.Exceeded;
	}

	return PlanCompletionStatus.NotCompleted;
};
