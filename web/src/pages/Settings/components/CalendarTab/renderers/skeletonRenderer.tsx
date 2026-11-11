import { Skeleton } from '@mui/material';

import { SkeletonCellWrapper } from './styles';

export const skeletonRenderer = () => (
	<SkeletonCellWrapper>
		<Skeleton />
	</SkeletonCellWrapper>
);
