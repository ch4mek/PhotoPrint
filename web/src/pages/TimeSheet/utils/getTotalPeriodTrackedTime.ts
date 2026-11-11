import { TimeTrackItem } from '../components/TimesheetEngineer/types';

export const getTotalPeriodTrackedTime = (
	timeMap: Map<string, TimeTrackItem | undefined>
) =>
	Array.from(timeMap.values()).reduce<[number, number]>(
		(acc, item) => {
			if (!item) {
				return acc;
			}

			const newPeriodTotalTime = acc;
			const { factTrackedInMinutes, billableTrackedInMinutes } = item;

			if (factTrackedInMinutes) {
				newPeriodTotalTime[0] += factTrackedInMinutes;
			}

			if (billableTrackedInMinutes) {
				newPeriodTotalTime[1] += billableTrackedInMinutes;
			}

			return newPeriodTotalTime;
		},
		[0, 0]
	);
