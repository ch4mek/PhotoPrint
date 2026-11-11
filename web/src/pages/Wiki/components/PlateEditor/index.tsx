import { useTheme } from '@emotion/react';
import { Plate } from '@udecode/plate';
import { useCallback, useMemo } from 'react';

import { useWikiContext } from '../../context/wikiContext';
import { EmptyArticleFiller } from '../EmptyArticleFiller';
import { KeywordsSection } from '../KeywordsSection';

import { renderLeaf } from '@/components/PlateToolbars/components/renderers/renderLeaf';
import { normalizeColorsToLightTheme } from '@/components/PlateToolbars/utils/normalizeColorsToLightTheme';
import { getDefaultColor, EDITOR_ID } from '@/constants/plateEditor';
import { handleEditorKeyDown } from '@/helpers/handleEditorKeyDown';
import { normalizeEditorData } from '@/helpers/normalizeEditorData';
import { useEditorState } from '@/hooks/useEditorState';
import { PlateEditorHeaderToolbar } from '@/pages/Wiki/components/PlateEditor/components/PlateEditorHeaderToolbar';
import { PlateEditorProps } from '@/pages/Wiki/components/PlateEditor/types';
import { splitKeywords } from '@/pages/Wiki/utils/splitKeywords';
import { getCustomPluginsWithThemeMode } from '@/plugins/PlateEditorPlugins';
import { CONFIG } from '@/plugins/PlateEditorPlugins/config';
import { useChangeArticle } from '@/queries/wiki/useChangeArticle';
import { CustomValue } from '@/types/plateEditor';

import { PlateSectionHeader } from './components/PlateSectionHeader';
import { ArticleTitle, EditorWrapper, ScrollbarWrapper } from './styles';

export function PlateEditor({
	currentWikiPage,
	currentArticleData,
	isEmptyArticle,
	refetch,
}: PlateEditorProps) {
	const theme = useTheme();
	const { isEditorReadOnly, setToggleEditorReadOnly } = useWikiContext();
	const { editorData, setEditorData, resetEditor } =
		useEditorState(currentWikiPage);
	const customPlugins = getCustomPluginsWithThemeMode(theme.mode);

	const { mutate: changeArticleMutation } = useChangeArticle({
		onSuccess: () => {
			refetch();
			setToggleEditorReadOnly();
		},
	});

	const { keywords } = currentArticleData;

	const isEditorEmpty = isEmptyArticle && isEditorReadOnly;

	const isShowKeywords = keywords && isEditorReadOnly && !isEditorEmpty;

	const handleReadButtonClick = useCallback(() => {
		if (!isEditorReadOnly) {
			resetEditor();
		}

		setToggleEditorReadOnly();
	}, [isEditorReadOnly, resetEditor, setToggleEditorReadOnly]);

	const handleEditorChange = (value: CustomValue) => {
		setEditorData(value);
	};

	const handleSaveButtonClick = useCallback(() => {
		if (!currentArticleData) {
			return;
		}

		const {
			id,
			articleReferences,
			name,
			groupId,
			isActive,
			isAvailableOnMainScreen,
			orderWeight,
			icon,
		} = currentArticleData;

		const data = normalizeColorsToLightTheme(normalizeEditorData(editorData));
		const text = JSON.stringify(data, null, 2);

		changeArticleMutation({
			id,
			groupId,
			name,
			text,
			keywords,
			isActive,
			isAvailableOnMainScreen,
			orderWeight,
			icon,
			articleReferences,
			articleTags: [],
		});
	}, [currentArticleData, editorData]);

	const defaultColor = getDefaultColor(theme.mode);

	const renderLeafNode = useMemo(
		() => renderLeaf(theme, defaultColor),
		[theme, defaultColor]
	);

	return (
		<>
			{isEditorReadOnly ? (
				<PlateSectionHeader
					title={currentArticleData.name}
					editorData={editorData}
					isEditorEmpty={isEditorEmpty}
					setToggleEditorReadOnly={setToggleEditorReadOnly}
				/>
			) : (
				<PlateEditorHeaderToolbar
					handleSaveButtonClick={handleSaveButtonClick}
					handleReadButtonClick={handleReadButtonClick}
				/>
			)}

			{isEditorEmpty ? (
				<EmptyArticleFiller />
			) : (
				<ScrollbarWrapper isEditor={!isEditorReadOnly}>
					<EditorWrapper isEditor={!isEditorReadOnly}>
						{!isEditorReadOnly && (
							<ArticleTitle variant="large">
								{currentArticleData.name}
							</ArticleTitle>
						)}
						<Plate<CustomValue>
							id={EDITOR_ID}
							editableProps={{
								...CONFIG.editableProps,
								readOnly: isEditorReadOnly,
								onKeyDown: (e) => handleEditorKeyDown(e, EDITOR_ID),
								renderLeaf: renderLeafNode,
								style: {
									...CONFIG.editableProps.style,
									color: defaultColor,
								},
							}}
							initialValue={currentWikiPage}
							value={editorData}
							onChange={handleEditorChange}
							plugins={customPlugins}
							normalizeInitialValue={false}
						/>
						{isShowKeywords && (
							<KeywordsSection keywords={splitKeywords(keywords)} />
						)}
					</EditorWrapper>
				</ScrollbarWrapper>
			)}
		</>
	);
}
