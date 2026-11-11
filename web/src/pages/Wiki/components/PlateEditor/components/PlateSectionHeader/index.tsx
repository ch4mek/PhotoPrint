import { useTheme } from '@emotion/react';
import { Button } from '@mms/mms-ui-library/components/Button';

import { ArticleExportControl } from '../ArticleExportControl';

import { PageTitleTypography } from '@/components/PageTitleTypography';
import { controlsText } from '@/constants/controlsTexts';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { PlateSectionHeaderProps } from '@/pages/Wiki/components/PlateEditor/components/PlateSectionHeader/types';
import { UserClaims } from '@/types/claims';

import {
	PlateControlsWrapper,
	PlateHeaderWrapper,
	PlateTitleWrapper,
} from './styles';

export function PlateSectionHeader({
	title,
	setToggleEditorReadOnly,
	editorData,
	isEditorEmpty,
}: PlateSectionHeaderProps) {
	const theme = useTheme();

	const { claims } = useAuth();

	const isDisplayArticleExportControl =
		checkIsHasClaims([UserClaims.ManageWikiExport], claims) && !isEditorEmpty;

	return (
		<PlateHeaderWrapper>
			<PlateTitleWrapper>
				<PageTitleTypography
					title={title}
					color={theme.palette['text-black']}
				/>
			</PlateTitleWrapper>
			<PlateControlsWrapper>
				{checkIsHasClaims([UserClaims.ManageArticle], claims) && (
					<Button
						isFullWidth={false}
						color="secondary"
						onClick={setToggleEditorReadOnly}
					>
						{controlsText.editText}
					</Button>
				)}
				{isDisplayArticleExportControl && (
					<ArticleExportControl title={title} editorData={editorData} />
				)}
			</PlateControlsWrapper>
		</PlateHeaderWrapper>
	);
}
