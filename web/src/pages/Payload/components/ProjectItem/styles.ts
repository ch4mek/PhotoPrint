import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { NBSP } from '../../contants';

import { ColorCircle } from '@/components/Grid/renderers/cell/cellWithColorCircleRenderer/styles';

import { ProjectDirectionProps } from './types';

const COLOR_CIRCLE_AREA_WIDTH = '15px';

export const ProjectsField = styled('div')<ProjectDirectionProps>(
	({ direction }) => ({
		display: 'grid',
		alignItems: 'center',

		...(direction === 'row'
			? {
					gridTemplateColumns: `${COLOR_CIRCLE_AREA_WIDTH} repeat(2, fit-content(50%))`,
					gridTemplateRows: '1fr',
					gridTemplateAreas: `'color client project'`,
			  }
			: {
					height: 34,
					gridTemplateColumns: `${COLOR_CIRCLE_AREA_WIDTH} auto`,
					gridTemplateRows: '1fr 1fr',
					gridTemplateAreas: `'color client' 'none project'`,
			  }),
	})
);

export const ColorCircleWrapper = styled(ColorCircle)(({ theme }) => ({
	gridArea: 'color',
	marginRight: theme.spaces.xs,
}));

const BaseProjectItemInfoField = styled(Typography)({
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	maxWidth: '100%',
});

export const ProjectItemClientNameField = styled(BaseProjectItemInfoField)(
	({ theme }) => ({
		color: theme.palette['text-black'],
		gridArea: 'client',
	})
);

export const ProjectItemProjectNameField = styled(
	BaseProjectItemInfoField
)<ProjectDirectionProps>(({ theme, direction }) => ({
	gridArea: 'project',
	color: theme.palette['text-gray'],

	'::before': {
		display: direction === 'column' ? 'none' : 'inline',
		content: `"${NBSP}/${NBSP}"`,
	},
}));
