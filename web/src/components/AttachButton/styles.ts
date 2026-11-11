import styled from '@emotion/styled';
import { PickerButton } from '@mms/mms-ui-library/components/PickerButton';

import { DEFAULT_DISABLE_OPACITY } from '@/constants/opacities';

const ATTACHED_FILE_BUTTON_MAX_WIDTH = 188;

export const AttachFileButtonWrapper = styled('div')<{ disabled?: boolean }>(
	({ theme, disabled }) => ({
		display: 'flex',
		alignItems: 'center',
		gap: theme.spaces.xs,
		cursor: 'pointer',
		maxWidth: ATTACHED_FILE_BUTTON_MAX_WIDTH,

		...(disabled && {
			opacity: DEFAULT_DISABLE_OPACITY,
			pointerEvents: 'none',
		}),
	})
);

export const HiddenInput = styled('input')({});

export const AttachPickerButton = styled(PickerButton)({
	width: 'fit-content',
});
