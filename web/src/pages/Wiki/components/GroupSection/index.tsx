import { CollapseGroup } from '@mms/mms-ui-library/components/CollapseGroup';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { DepartmentIcon } from '@mms/mms-ui-library/icons/base/DepartmentIcon';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { ProjectsIcon } from '@mms/mms-ui-library/icons/base/ProjectsIcon';
import {
	HTMLAttributes,
	MouseEvent,
	useCallback,
	useContext,
	useEffect,
	useMemo,
	useState,
} from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

import { useWikiContext } from '../../context/wikiContext';

import { MenuItem } from '@/components/DraggableControlIcon/types';
import { Loader } from '@/components/Loader';
import { NoDataFoundWrapper } from '@/components/NoDataFoundWrapper';
import { Routes } from '@/constants/routes';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { getIdFromPathname } from '@/helpers/getIdFromPathname';
import { ArticleForm } from '@/pages/Wiki/components/GroupSection/components/ArticleForm';
import { ArticleFormValues } from '@/pages/Wiki/components/GroupSection/components/ArticleForm/types';
import { CollapseItem } from '@/pages/Wiki/components/GroupSection/components/CollapseItem';
import { CollapseItemWrapper } from '@/pages/Wiki/components/GroupSection/components/CollapseItemWrapper';
import { DeleteConfirmationDialog } from '@/pages/Wiki/components/GroupSection/components/DeleteConfirmationDialog';
import { DraggableItem } from '@/pages/Wiki/components/GroupSection/components/DraggableItem';
import { DraggablePreviewItem } from '@/pages/Wiki/components/GroupSection/components/DraggablePreviewItem';
import { GroupControls } from '@/pages/Wiki/components/GroupSection/components/GroupControls';
import { GroupForm } from '@/pages/Wiki/components/GroupSection/components/GroupForm';
import { GroupFormValues } from '@/pages/Wiki/components/GroupSection/components/GroupForm/types';
import { StaticItem } from '@/pages/Wiki/components/GroupSection/components/StaticItem';
import { NO_ARTICLE_DATA_TITLE } from '@/pages/Wiki/components/GroupSection/constants';
import { useDeleteSegmentForm } from '@/pages/Wiki/components/GroupSection/hooks/useDeleteKnowledgeGroupForm';
import { useGroupControls } from '@/pages/Wiki/components/GroupSection/hooks/useGroupControls';
import { useUpdateSegmentForm } from '@/pages/Wiki/components/GroupSection/hooks/useUpdateKnowledgeGroupForm';
import { useUpdateOrder } from '@/pages/Wiki/components/GroupSection/hooks/useUpdateOrder';
import { CollapseWrapperProps } from '@/pages/Wiki/components/GroupSection/types';
import { DraggableContext } from '@/pages/Wiki/context/draggableContext';
import { Option } from '@/pages/Wiki/types';
import {
	DEFAULT_ADD_ARTICLE_PARAMS,
	DEFAULT_ADD_GROUP_PARAMS,
} from '@/queries/wiki/constants';
import {
	AddArticleParams,
	AddGroupParams,
	FullArticleItem,
	KnowledgeGroupItem,
	UpdateArticleParams,
	UpdateGroupNameParam,
} from '@/queries/wiki/types';
import { useGetKnowledgeGroups } from '@/queries/wiki/useGetKnowledgeGroups';
import { useSearchArticle } from '@/queries/wiki/useSearchArticle';
import { UserClaims } from '@/types/claims';
import { SegmentTypes } from '@/types/wiki';

import { useAddSegmentForm } from './hooks/useAddKnowledgeGroupForm';
import {
	CollapseGroupWrapper,
	GroupSectionWrapper,
	GroupWrapper,
} from './styles';
import {
	convertArticlesToOptions,
	getIdFromUniqueGroupsIds,
	serializeOptions,
	transformOrderStructure,
} from './utils';

export function GroupSection() {
	const location = useLocation();
	const { isEditorReadOnly } = useWikiContext();
	const navigate = useNavigate();
	const selectedItemId = getIdFromPathname(location.pathname);
	const [segmentType, setSegmentType] = useState<SegmentTypes | null>(null);
	const [deletedTitle, setDeletedTitle] = useState('');
	const [isMoveMode, setIsMoveMode] = useState(false);
	const { setCollapseData, collapseData } = useContext(DraggableContext);

	const { claims: userClaims } = useAuth();

	const {
		debouncedSearchValue,
		searchValue,
		handleSearchChange,
		handleSearchReset,
	} = useGroupControls();

	const { data: searchedArticles, isLoading: isSearchArticleLoading } =
		useSearchArticle({
			name: debouncedSearchValue,
		});

	const {
		data: knowledgeGroups,
		isLoading,
		isFetching,
	} = useGetKnowledgeGroups();

	const serializedKnowledgeGroups = useMemo(
		() => serializeOptions(knowledgeGroups),
		[knowledgeGroups]
	);

	useEffect(() => {
		if (debouncedSearchValue.trim()) {
			setCollapseData({
				items: convertArticlesToOptions(searchedArticles || []),
			});
		} else {
			setCollapseData(serializedKnowledgeGroups);
		}
	}, [serializedKnowledgeGroups, debouncedSearchValue, searchedArticles]);

	const {
		addSegmentFormId,
		handleAddSegmentFormOpen,
		handleAddSegmentFormClose,
		isAddSegmentFormOpen,
		addSegmentMutation,
	} = useAddSegmentForm({ segmentType });

	const {
		updateSegmentFormId,
		handleUpdateSegmentFormOpen,
		handleUpdateSegmentFormClose,
		isUpdateSegmentFormOpen,
		updateSegmentMutation,
	} = useUpdateSegmentForm({ segmentType });

	const {
		deleteSegmentGroupFormId,
		isDeleteSegmentFormOpen,
		handleDeleteSegmentFormOpen,
		handleDeleteSegmentFormClose,
		deleteSegmentMutation,
		isDeleteSegmentLoading,
	} = useDeleteSegmentForm({ segmentType });

	const handleGroupMutateSubmit = useCallback(
		(values: GroupFormValues) => {
			const isEditMode = updateSegmentFormId != null;
			const data: AddGroupParams | UpdateGroupNameParam = isEditMode
				? ({
						id: updateSegmentFormId,
						...values,
				  } as UpdateGroupNameParam)
				: ({
						...DEFAULT_ADD_GROUP_PARAMS,
						...values,
						parentGroupId: addSegmentFormId ?? null,
				  } as AddGroupParams);

			const submitFunction = (
				isEditMode ? updateSegmentMutation : addSegmentMutation
			) as (
				data: AddGroupParams | UpdateGroupNameParam
			) => Promise<KnowledgeGroupItem>;

			return submitFunction(data);
		},
		[
			updateSegmentFormId,
			addSegmentFormId,
			addSegmentMutation,
			updateSegmentMutation,
		]
	);

	const handleDeleteSegmentMutateSubmit = useCallback(async () => {
		if (deleteSegmentGroupFormId) {
			await deleteSegmentMutation(deleteSegmentGroupFormId);
		}
	}, [deleteSegmentGroupFormId]);

	const handleAddGroupClick = useCallback(
		(type: SegmentTypes, parentGroupId?: string | number) => () => {
			setSegmentType(type);
			handleAddSegmentFormOpen(parentGroupId);
		},
		[handleAddSegmentFormOpen]
	);

	const handleUpdateGroupClick = useCallback(
		(type: SegmentTypes, groupId?: string | number) => {
			setSegmentType(type);
			handleUpdateSegmentFormOpen(groupId);
		},
		[handleUpdateSegmentFormOpen]
	);

	const handleDeleteSegmentClick = useCallback(
		(type: SegmentTypes, title: string, groupId?: string | number) => {
			setSegmentType(type);
			setDeletedTitle(title);
			handleDeleteSegmentFormOpen(groupId);
		},
		[handleDeleteSegmentFormOpen]
	);

	const handleArticleSubmit = useCallback(
		(values: FullArticleItem | ArticleFormValues) => {
			const isEditMode = updateSegmentFormId != null;
			const data = isEditMode
				? ({
						id: updateSegmentFormId,
						...values,
						articleTags: 'tags' in values ? values.tags : [],
				  } as UpdateArticleParams)
				: ({
						...DEFAULT_ADD_ARTICLE_PARAMS,
						...values,
						groupId: addSegmentFormId ?? null,
				  } as AddArticleParams);

			const submitFunction = (
				isEditMode ? updateSegmentMutation : addSegmentMutation
			) as (
				data: AddArticleParams | UpdateArticleParams
			) => Promise<FullArticleItem>;

			return submitFunction(data);
		},
		[
			updateSegmentFormId,
			addSegmentFormId,
			updateSegmentMutation,
			addSegmentMutation,
		]
	);

	const handleContentClick = useCallback(
		(_e: MouseEvent<HTMLElement>, data: Option) => {
			const id = getIdFromUniqueGroupsIds(data.id);
			navigate(`${Routes.wiki}/${id}`);
		},
		[]
	);

	const handleCloseFunction = useMemo(
		() =>
			updateSegmentFormId != null
				? handleUpdateSegmentFormClose
				: handleAddSegmentFormClose,
		[updateSegmentFormId]
	);

	const handleModeButtonClick = useCallback(() => {
		setIsMoveMode((prevState) => !prevState);
	}, []);

	const { updateGroupOrderMutation, isUpdateGroupOrderLoading } =
		useUpdateOrder();

	const handleUpdateOrder = useCallback(async () => {
		const orderData = transformOrderStructure(collapseData.items);
		await updateGroupOrderMutation({ groupOrders: orderData });
		setIsMoveMode(false);
	}, [collapseData]);

	const getCollapseItemMenuOption = useCallback(
		(type: SegmentTypes) =>
			[
				{
					value: 'Add article',
					addon: <ProjectsIcon />,
					claims: [UserClaims.ManageArticle],
					onClick: ({ id }: MenuItem) =>
						handleAddGroupClick(SegmentTypes.Article, id)(),
				},
				{
					value: 'Add subgroup',
					addon: <DepartmentIcon />,
					claims: [UserClaims.ManageSubgroup],
					onClick: ({ id }: MenuItem) =>
						handleAddGroupClick(SegmentTypes.SubGroup, id)(),
				},
				{
					value: `Edit ${type.toLowerCase()}`,
					addon: <EditIcon />,
					claims:
						type === SegmentTypes.Group
							? [UserClaims.ManageGroup]
							: [UserClaims.ManageSubgroup],
					onClick: ({ id }: MenuItem) => handleUpdateGroupClick(type, id),
				},
				{
					value: `Delete ${type.toLowerCase()}`,
					addon: <BasketIcon />,
					claims:
						type === SegmentTypes.Group
							? [UserClaims.ManageGroup]
							: [UserClaims.ManageSubgroup],
					onClick: ({ id, name }: MenuItem) =>
						handleDeleteSegmentClick(type, name, id),
				},
			].filter(({ claims }) => checkIsHasClaims(claims, userClaims)),
		[]
	);

	const staticItemMenuOption = useMemo(
		() => [
			{
				value: 'Edit',
				addon: <EditIcon />,
				onClick: ({ id }: MenuItem) =>
					handleUpdateGroupClick(SegmentTypes.Article, id),
			},
			{
				value: 'Delete',
				addon: <BasketIcon />,
				onClick: ({ id, name }: MenuItem) =>
					handleDeleteSegmentClick(SegmentTypes.Article, name, id),
			},
		],
		[]
	);

	const toggle = useCallback((item: Option) => {
		setCollapseData((prevGroups) => {
			const updateGroups = (groups: Option[]): Option[] =>
				groups.map((group) => {
					if (group.id === item.id && group.isGroup) {
						return { ...group, expanded: !group.expanded };
					}

					if (group.items) {
						return { ...group, items: updateGroups(group.items) };
					}

					return group;
				});

			return {
				items: updateGroups(prevGroups.items || []),
			};
		});
	}, []);

	const collapseWrapper = useCallback(
		<T,>(props: CollapseWrapperProps<Option> & HTMLAttributes<T>) => (
			<CollapseItemWrapper {...props} isDragMode={isMoveMode} />
		),
		[isMoveMode]
	);

	const isShowLoader =
		isLoading ||
		isFetching ||
		isUpdateGroupOrderLoading ||
		isSearchArticleLoading;

	if (!isEditorReadOnly) {
		return null;
	}

	return (
		<>
			<GroupWrapper>
				<GroupSectionWrapper>
					<GroupControls
						{...{
							handleSearchReset,
							searchValue,
							handleSearchChange,
							handleAddGroupClick,
						}}
						handleModeButtonClick={
							isMoveMode ? handleUpdateOrder : handleModeButtonClick
						}
						isMoveModeEnable={isMoveMode}
						isButtonsDisabled={isShowLoader || !!debouncedSearchValue}
					/>
					<CollapseGroupWrapper>
						<Loader isLoading={isShowLoader}>
							<NoDataFoundWrapper
								isEmptyData={
									(!collapseData.items || collapseData.items.length === 0) &&
									!isShowLoader
								}
								title={NO_ARTICLE_DATA_TITLE}
							>
								<CollapseGroup
									options={collapseData.items as Option[]}
									unMountOnExit
									onContentClick={handleContentClick}
									collapseGroupWrapper={collapseWrapper}
									toggleCollapse={toggle}
									renderCollapseItem={({
										disabled,
										expanded,
										item,
										onClick,
									}) => (
										<CollapseItem
											id={item.id}
											onClick={onClick}
											disabled={disabled}
											expanded={!!expanded}
											getMenuItems={getCollapseItemMenuOption}
											title={item?.name || ''}
											parentGroupId={item?.parentGroupId}
											isMoveMode={isMoveMode}
										/>
									)}
									renderStaticItem={({ onClick, item, disabled }) => (
										<DraggableItem item={item!} isDragMode={isMoveMode}>
											<StaticItem
												item={item}
												onClick={onClick}
												menuOptions={staticItemMenuOption}
												isSelected={
													getIdFromUniqueGroupsIds(item.id) === +selectedItemId
												}
												isMoveMode={isMoveMode}
												disabled={disabled}
											/>
										</DraggableItem>
									)}
								/>
							</NoDataFoundWrapper>
						</Loader>
					</CollapseGroupWrapper>
				</GroupSectionWrapper>
				<DraggablePreviewItem />
			</GroupWrapper>

			{(isAddSegmentFormOpen || isUpdateSegmentFormOpen) &&
				segmentType &&
				segmentType !== SegmentTypes.Article && (
					<GroupForm
						type={segmentType}
						handleSubmit={handleGroupMutateSubmit}
						handleClose={handleCloseFunction}
						groupId={updateSegmentFormId}
					/>
				)}

			{(isAddSegmentFormOpen || isUpdateSegmentFormOpen) &&
				segmentType === SegmentTypes.Article && (
					<ArticleForm
						handleSubmit={handleArticleSubmit}
						handleClose={handleCloseFunction}
						articleId={updateSegmentFormId}
					/>
				)}

			{isDeleteSegmentFormOpen && segmentType && (
				<DeleteConfirmationDialog
					handleCloseDialog={handleDeleteSegmentFormClose}
					handleDeleteSegment={handleDeleteSegmentMutateSubmit}
					showLoader={isDeleteSegmentLoading}
					segmentType={segmentType}
					title={deletedTitle}
				/>
			)}
		</>
	);
}
