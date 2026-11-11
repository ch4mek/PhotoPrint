import { Skeleton } from '@mms/mms-ui-library/components/Skeleton';
import { RenderCellProps } from 'react-data-grid';

import { CellWrapper } from '../../styles';
import { GridCellDefaultProps } from '../../types';

import { SKELETON_FILL_WITH_PADDING } from '@/pages/TimeSheet/components/TimeSheeetManager/constants';

import { FullAccessIcon, NoAccessIcon, PartialAccessIcon } from './styles';
import { ClaimsGroupStatus } from './types';

export const claimCellRenderer = <T extends GridCellDefaultProps>({
	row,
	column: { key },
}: RenderCellProps<T, unknown>) => {
	const isActive = row[key as keyof T] as boolean;

	const { showSkeleton, disabled } = row;

	if (isActive === undefined) {
		return <CellWrapper disabled={disabled} />;
	}

	const showStatus = () => {
		switch (row[key as keyof T]) {
			case ClaimsGroupStatus.NoAccess: {
				return <NoAccessIcon />;
			}
			case ClaimsGroupStatus.FullAccess: {
				return <FullAccessIcon />;
			}
			case ClaimsGroupStatus.PartialAccess: {
				return <PartialAccessIcon />;
			}
			default: {
				return <NoAccessIcon />;
			}
		}
	};

	return showSkeleton ? (
		<Skeleton
			width={SKELETON_FILL_WITH_PADDING}
			height={SKELETON_FILL_WITH_PADDING}
			visible
		/>
	) : (
		showStatus()
	);
};
