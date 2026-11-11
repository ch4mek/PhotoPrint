import { useTheme } from '@emotion/react';
import { TransactionIcon } from '@mms/mms-ui-library/icons/base';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { MerchKitsGridRow } from '../../../KitsTab/types';
import { ColoredEditCellWrapper } from '../../styles';
import { MerchStorageGridRow } from '../../types';
import { getCellColorForRow } from '../../utils/getCellColorForRow';

import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { MerchStatuses } from '@/pages/Merch/constants';
import { UserClaims } from '@/types/claims';

export function ColoredTransactionRenderCell({
	row,
}: RenderCellProps<
	MerchKitsGridRow & Omit<MerchStorageGridRow, keyof MerchKitsGridRow>
>) {
	const { claims } = useAuth();
	const theme = useTheme();

	const cellColor = useMemo(
		() => getCellColorForRow(row.sizeAmounts, theme, row.expanded),
		[row, theme]
	);

	const isArchived = row.status === MerchStatuses.archived;

	if (
		checkIsHasClaims([UserClaims.ManageMerchTransactions], claims) &&
		!isArchived
	) {
		return (
			<ColoredEditCellWrapper color={cellColor}>
				<TransactionIcon
					onClick={row.handleEditClick(row.id)}
					color={theme.palette['graphics-primary-2']}
				/>
			</ColoredEditCellWrapper>
		);
	}

	return <ColoredEditCellWrapper color={cellColor} />;
}
