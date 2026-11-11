import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { CellWrapper } from '../../styles';
import { GridCellDefaultProps } from '../../types';

import { TimeAgoComponent } from './TimeAgo';

export const cellWithLastUpdateRenderer = <T extends GridCellDefaultProps>(
	props: RenderCellProps<T, unknown>,
	lastUpdate: string | number
): React.ReactNode => {
	const { row } = props;
	const isLoading = row.showSkeleton;

	return isLoading ? (
		<Skeleton animation="wave" width="100%" />
	) : (
		<CellWrapper disabled={row.disabled}>
			{lastUpdate ? <TimeAgoComponent time={lastUpdate} /> : ''}
		</CellWrapper>
	);
};
