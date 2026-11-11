import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { CellWrapper } from '../../styles';
import { GridCellDefaultProps } from '../../types';

import { monthsToYearsAndMonthsObject } from '@/helpers/translateDateUtils';

export function createProfExperienceCellRender<T extends GridCellDefaultProps>(
	getProfExperienceInMonth: (row: T) => number | null
) {
	return function (props: RenderCellProps<T, unknown>) {
		const { row } = props;

		const { showSkeleton } = row;

		const profExperienceInMonth = getProfExperienceInMonth(row);

		if (profExperienceInMonth === null) {
			return showSkeleton ? (
				<Skeleton animation="wave" width="100%" />
			) : (
				<CellWrapper />
			);
		}

		const { years, months } = monthsToYearsAndMonthsObject(
			profExperienceInMonth > 0 ? profExperienceInMonth : 0
		);

		return showSkeleton ? (
			<Skeleton animation="wave" width="100%" />
		) : (
			<CellWrapper>
				{years ? (
					<>
						{years} y {months} m
					</>
				) : (
					<>{months} m</>
				)}
			</CellWrapper>
		);
	};
}
