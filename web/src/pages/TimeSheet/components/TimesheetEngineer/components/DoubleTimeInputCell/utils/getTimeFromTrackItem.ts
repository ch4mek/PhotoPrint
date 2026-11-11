import { TimeTrackItem } from '@/pages/TimeSheet/components/TimesheetEngineer/types';
import { TrackedTimeType } from '@/types/index';

export const getTimeFromTrackItem = (
	trackItem: TimeTrackItem | undefined,
	inputTimeType: TrackedTimeType
) => {
	if (!trackItem) {
		return 0;
	}

	if (inputTimeType === TrackedTimeType.Billable) {
		return trackItem.billableTrackedInMinutes ?? 0;
	}

	return trackItem.factTrackedInMinutes ?? 0;
};
