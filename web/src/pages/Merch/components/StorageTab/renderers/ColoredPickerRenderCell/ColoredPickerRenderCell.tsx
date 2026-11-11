import { useTheme } from '@emotion/react';
import {
	PickerButton,
	PickerButtonOption,
} from '@mms/mms-ui-library/components/PickerButton';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { TransactionIcon } from '@mms/mms-ui-library/icons/base/TransactionIcon';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { MerchKitsGridRow } from '../../../KitsTab/types';
import { ColoredEditCellWrapper } from '../../styles';
import { MerchStorageGridRow } from '../../types';
import { getCellColorForRow } from '../../utils/getCellColorForRow';

import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import {
	MerchEditCellOptionValues,
	MerchStatuses,
} from '@/pages/Merch/constants';
import { UserClaims } from '@/types/claims';

export function ColoredPickerRenderCell({
	row,
}: RenderCellProps<
	MerchKitsGridRow & Omit<MerchStorageGridRow, keyof MerchKitsGridRow>
>) {
	const { claims } = useAuth();
	const canManageItems = checkIsHasClaims(
		[UserClaims.ManageItemsAndKits],
		claims
	);
	const canManageMerchTransactions = checkIsHasClaims(
		[UserClaims.ManageMerchTransactions],
		claims
	);

	const theme = useTheme();
	const isArchived = row.status === MerchStatuses.archived;

	const menuOptions = useMemo<Array<PickerButtonOption>>(
		() => [
			...(canManageMerchTransactions && !isArchived
				? [
						{
							addon: <TransactionIcon />,
							onClick: row.handleTransactionAdd
								? row.handleTransactionAdd(row.id)
								: () => {},
							value: MerchEditCellOptionValues.transaction,
						},
				  ]
				: []),
			...(canManageItems
				? [
						{
							addon: <EditIcon />,
							onClick: row.handleEdit(row.id),
							value: MerchEditCellOptionValues.edit,
						},
				  ]
				: []),
		],
		[row, canManageItems, canManageMerchTransactions, isArchived]
	);

	const cellColor = useMemo(
		() => getCellColorForRow(row.possibleKits, theme, row.expanded, isArchived),
		[row, theme, isArchived]
	);

	if (menuOptions.length === 0) {
		return <ColoredEditCellWrapper color={cellColor} disabled={isArchived} />;
	}

	return (
		<ColoredEditCellWrapper color={cellColor} disabled={isArchived}>
			<PickerButton
				icon={<OptionDotsIcon />}
				options={menuOptions}
				item={row}
				popoverPosition="left-start"
			/>
		</ColoredEditCellWrapper>
	);
}
