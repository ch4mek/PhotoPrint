import styled from '@emotion/styled';
import { Avatar } from '@mms/mms-ui-library/components/Avatar';

const AVATAR_SIZE = 68;

export const ProfileAvatarSquare = styled(Avatar)(() => ({
	width: AVATAR_SIZE,
	height: AVATAR_SIZE,
	cursor: 'pointer',

	'& img': {
		width: AVATAR_SIZE,
		height: AVATAR_SIZE,
	},
}));

export const ProfileAvatarWrapper = styled('div')({
	width: 'max-content',
	display: 'flex',

	'& button': {
		height: 'auto',
		width: 'auto',
	},
});
