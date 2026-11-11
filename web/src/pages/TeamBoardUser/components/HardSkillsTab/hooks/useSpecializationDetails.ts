import { useMemo } from 'react';

import { areDatesEqual } from '@/helpers/areValuesEqual';
import dayjs from '@/lib/dayjs';
import { SnapshotDateConfig } from '@/pages/TeamBoardUser/components/SpecializationTable/types';
import { UserSpecializationInfo } from '@/types/teamboard';

export const useSpecializationDetails = (
	specializations?: UserSpecializationInfo[]
) => {
	const primarySpecializationId = useMemo(
		() =>
			specializations?.find((specialization) => specialization.isPrimary)?.id,
		[specializations]
	);

	const isSpecializationsEmpty = useMemo(
		() => specializations && specializations.length === 0,
		[specializations]
	);

	const allDates: SnapshotDateConfig[] = useMemo(() => {
		if (!specializations) {
			return [];
		}

		const snapshotMap = specializations.reduce<
			Record<string, SnapshotDateConfig>
		>((acc, { specializationSnapshots }) => {
			specializationSnapshots.forEach(({ snapshotDate, id }) => {
				if (!acc[snapshotDate]) {
					acc[snapshotDate] = {
						snapshotDate,
						relatedSnapshotId: id,
					};
				}
			});

			return acc;
		}, {});

		return Object.values(snapshotMap).sort((a, b) =>
			b.snapshotDate.localeCompare(a.snapshotDate)
		);
	}, [specializations]);

	const isLastSnapshotDateEqualCurrentDate = useMemo(
		() =>
			allDates &&
			allDates.length > 0 &&
			areDatesEqual(dayjs(allDates[0].snapshotDate).toDate(), new Date()),
		[allDates]
	);

	const lastSnapshotDate = useMemo(() => {
		if (allDates && allDates.length > 0) {
			return dayjs(allDates[0].snapshotDate).add({ day: 1 }).toDate();
		}
	}, [allDates]);

	return {
		primarySpecializationId,
		isSpecializationsEmpty,
		allDates,
		isLastSnapshotDateEqualCurrentDate,
		lastSnapshotDate,
	};
};
