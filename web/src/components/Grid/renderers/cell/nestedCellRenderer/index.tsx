import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { CellWrapper } from '../../styles';
import { GridCellDefaultProps } from '../../types';

import { Content } from './styles';

const CLIENT_PROJECTS_CELL_PADDING_LEFT = 18;

export const nestedCellRenderer = <T extends GridCellDefaultProps>({
	row,
	column: { key },
}: RenderCellProps<T, unknown>) => {
	const info = row[key as keyof T] as string;

	const { showSkeleton, disabled } = row;

	return showSkeleton ? (
		<Skeleton animation="wave" width="100%" />
	) : (
		<CellWrapper
			disabled={disabled}
			padding={`0px 0px 0px ${CLIENT_PROJECTS_CELL_PADDING_LEFT}px`}
		>
			<Tooltip text={info} useIfTextCut>
				<Content variant="s-400">{info}</Content>
			</Tooltip>
		</CellWrapper>
	);
};
