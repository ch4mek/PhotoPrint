import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { PlusIcon } from '@mms/mms-ui-library/icons/base/PlusIcon';
import { memo } from 'react';

import { IconSize } from '@/constants/iconSizes';

import {
	Wrapper,
	HeaderWrapper,
	ButtonsWrapper,
	Title,
	ViewButton,
} from './styles';
import { InfoDataContainerProps } from './types';

function InfoDataContainerInner({
	headerTitle,
	handleEditClick,
	handleAddClick,
	children,
	handleClickView,
	tabId,
	orderName,
	isDisableAddButton = false,
}: InfoDataContainerProps) {
	return (
		<Wrapper orderName={orderName}>
			<HeaderWrapper>
				<Title variant="m-700">{headerTitle}</Title>
				<ButtonsWrapper>
					{handleEditClick && (
						<ActionButton onClick={handleEditClick} size="small">
							<EditIcon />
						</ActionButton>
					)}
					{handleAddClick && (
						<ActionButton
							disabled={isDisableAddButton}
							onClick={handleAddClick}
							size="small"
						>
							<PlusIcon height={IconSize.MS} width={IconSize.MS} />
						</ActionButton>
					)}

					{tabId && handleClickView && (
						<ViewButton onClick={handleClickView} size="small">
							<Typography variant="m-600">View</Typography>
						</ViewButton>
					)}
				</ButtonsWrapper>
			</HeaderWrapper>
			{children}
		</Wrapper>
	);
}

export const InfoDataContainer = memo(InfoDataContainerInner);
