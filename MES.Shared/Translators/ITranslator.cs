namespace MES.Shared;

public interface ITranslator<TEntity, TDTO>
{
    TDTO ToDTO(TEntity entity);
    TEntity ToEntity(TDTO dto);
    IEnumerable<TDTO> ToDTOs(IEnumerable<TEntity> entities);
    IEnumerable<TEntity> ToEntities(IEnumerable<TDTO> dtos);
}