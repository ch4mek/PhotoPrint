import styled from '@emotion/styled';
import { Input } from '@mms/mms-ui-library/components/Input';

const MIN_INPUT_HEIGHT = 40;

export const BookingGuestInput = styled(Input)(() => ({
	minHeight: `${MIN_INPUT_HEIGHT}px`,
}));
