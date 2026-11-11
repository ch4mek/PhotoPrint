import { PickerButton } from '@mms/mms-ui-library/components/PickerButton';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useRef, useState } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { useRowMenuOptions } from '../../hooks/useRowMenuOptions';
import { UserCertificatesRow } from '../../types';

import { confirmationMessage } from '@/components/DialogTypography/constants';
import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import { CERTIFICATES_QUERY_KEY } from '@/constants/queriesKeys';
import { zIndexes } from '@/constants/zIndexes';
import { useToast } from '@/context/Toast';
import { useAddScrollListener } from '@/hooks/useAddScrollListener';
import { useDeleteCertificate } from '@/queries/useDeleteCertificate';

import {
	DELETE_CERTIFICATE_CONFIRMATION_MESSAGE,
	POPUP_TITLE,
} from './constants';

export function CertificateCellOptions({
	row,
}: RenderCellProps<UserCertificatesRow>) {
	const toast = useToast();
	const { mutateAsync } = useDeleteCertificate(toast);
	const queryClient = useQueryClient();

	const [isConfirmationPopupVisible, setIsConfirmationPopupVisible] =
		useState(false);

	const pickerButtonRef = useRef<HTMLButtonElement>(null);
	const [isOpenPopover, setIsOpenPopover] =
		useAddScrollListener(pickerButtonRef);

	const handleConfirmationPopupToggle = () => {
		setIsConfirmationPopupVisible((prevVisible) => !prevVisible);
	};

	const handleRemoveCertificate = useCallback(async () => {
		await mutateAsync({ certificateId: row.certificateId, userId: row.userId });
		queryClient.invalidateQueries({ queryKey: [CERTIFICATES_QUERY_KEY] });
	}, [mutateAsync, row, queryClient]);

	const certificateMenuOptions = useRowMenuOptions({
		handleViewCertificate: () => window.open(row.certificateViewLink),
		handleRemoveCertificate: handleConfirmationPopupToggle,
	});

	return (
		<>
			<PickerButton
				icon={<OptionDotsIcon />}
				options={certificateMenuOptions}
				item={row}
				popoverPosition="left-start"
				isOpenPopover={isOpenPopover}
				setIsOpenPopover={setIsOpenPopover}
				ref={pickerButtonRef}
			/>
			{isConfirmationPopupVisible && (
				<PopUp
					title={POPUP_TITLE}
					onClose={handleConfirmationPopupToggle}
					type="approve"
					showCloseButton={false}
					controls={{
						positiveControl: { onClick: handleRemoveCertificate },
						negativeControl: { onClick: handleConfirmationPopupToggle },
					}}
					zIndex={zIndexes.formPopup}
				>
					<Typography variant="m-400">
						{DELETE_CERTIFICATE_CONFIRMATION_MESSAGE}
					</Typography>
					<ConfirmationTypography variant="m-400">
						{confirmationMessage}
					</ConfirmationTypography>
				</PopUp>
			)}
		</>
	);
}
