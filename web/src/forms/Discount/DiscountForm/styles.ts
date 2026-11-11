import styled from '@emotion/styled';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects/Autoselect';
import { Form } from 'formik';

import { TextField } from '@/fields/Text';
import { discountFieldData } from '@/forms/Discount/DiscountForm/constants';

const {
	mediaLinks,
	cities,
	tags,
	website,
	phoneNumber,
	companyType,
	discountPercent,
	discountPercentTo,
	discountCategory,
	name,
	location,
} = discountFieldData;

const MIDDLE_BREAKPOINT = 620;
const FINAL_BREAKPOINT = 832;

export const FormContent = styled(Form)(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
}));

export const FirstRowGrid = styled.div(({ theme }) => ({
	display: 'grid',
	columnGap: theme.spaces.s,
	rowGap: theme.spaces.l,
	gridTemplateColumns: `1fr 1fr`,
	gridTemplateAreas: `
    '${discountCategory.gridArea} ${name.gridArea}'
  `,

	[theme.breakpoints.down(MIDDLE_BREAKPOINT)]: {
		gridTemplateColumns: '1fr',
		gridTemplateAreas: `
    '${discountCategory.gridArea}'
    '${name.gridArea}'
  `,
	},
}));

export const SecondRowGrid = styled.div(({ theme }) => ({
	display: 'grid',
	columnGap: theme.spaces.s,
	rowGap: theme.spaces.l,
	gridTemplateColumns: `1fr 1fr 350px`,
	gridTemplateAreas: `
'${discountPercent.gridArea} ${discountPercentTo.gridArea} ${companyType.gridArea}' 
`,

	[theme.breakpoints.down(FINAL_BREAKPOINT)]: {
		gridTemplateColumns: 'repeat(2, 1fr)',
		gridTemplateAreas: `
	'${discountPercent.gridArea} ${discountPercentTo.gridArea}'
	'${companyType.gridArea} ${companyType.gridArea}'
	`,
	},

	[theme.breakpoints.down(MIDDLE_BREAKPOINT)]: {
		gridTemplateColumns: '1fr',
		gridTemplateAreas: `
	'${discountPercent.gridArea}'
	'${discountPercentTo.gridArea}'
	'${companyType.gridArea}'
	`,
	},
}));

export const RemainingFormGrid = styled.div(({ theme }) => ({
	display: 'grid',
	rowGap: theme.spaces.l,
	columnGap: theme.spaces.s,
	gridTemplateColumns: 'repeat(3, 1fr)',
	gridTemplateAreas: `
    '${website.gridArea} ${phoneNumber.gridArea} ${location.gridArea}'
    '${mediaLinks.gridArea} ${mediaLinks.gridArea} ${mediaLinks.gridArea}'
    '${cities.gridArea} ${cities.gridArea} ${cities.gridArea}'
    '${tags.gridArea} ${tags.gridArea} ${tags.gridArea}'
  `,

	[theme.breakpoints.down(theme.breakpoints.tablet)]: {
		gridTemplateColumns: 'repeat(2, 1fr)',
		gridTemplateAreas: `
    '${website.gridArea} ${phoneNumber.gridArea}'
    '${location.gridArea} ${location.gridArea}'
    '${mediaLinks.gridArea} ${mediaLinks.gridArea}'
    '${cities.gridArea} ${cities.gridArea}'
    '${tags.gridArea} ${tags.gridArea}'
  `,
	},

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		gridTemplateColumns: '1fr',
		gridTemplateAreas: `
    '${website.gridArea}'
    '${phoneNumber.gridArea}'
    '${location.gridArea}'
    '${mediaLinks.gridArea}'
    '${cities.gridArea} '
    '${tags.gridArea}'
  `,
	},
}));

export const CollapsableDataWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
	width: '100%',
	paddingRight: theme.spaces.xs + 1,
}));

export const TextFieldGridWrapper = styled(TextField)<{ gridArea: string }>(
	({ gridArea }) => ({
		gridArea,
	})
);

export const AutoSelectGridWrapper = styled(Autoselect)<{ gridArea: string }>(
	({ gridArea }) => ({
		gridArea,
	})
);

export const GridAreaWrapper = styled('div')<{ gridArea: string }>(
	({ gridArea }) => ({
		gridArea,
		width: '100%',
	})
);
