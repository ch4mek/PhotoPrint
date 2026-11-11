import { useTheme } from '@emotion/react';
import { CheckedIcon } from '@mms/mms-ui-library/icons/base/CheckedIcon';
import { CrossIcon } from '@mms/mms-ui-library/icons/base/CrossIcon';
import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '../../types';

import { LogicCellWrapper } from './styles';

export const logicCellRenderer = <T extends GridCellDefaultProps>({
	row,
	column: { key },
}: RenderCellProps<T, unknown>) => {
	const theme = useTheme();

	const isTrue = row[key as keyof T] as boolean;
	const { showSkeleton, disabled } = row;

	return showSkeleton ? (
		<Skeleton animation="wave" width="100%" />
	) : (
		<LogicCellWrapper disabled={disabled}>
			{isTrue ? (
				<CheckedIcon color={theme.palette['graphics-accent']} />
			) : (
				<CrossIcon color={theme.palette['graphics-light-gray']} />
			)}
		</LogicCellWrapper>
	);
};
